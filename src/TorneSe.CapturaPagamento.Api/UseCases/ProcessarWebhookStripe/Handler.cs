using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using TorneSe.CapturaPagamento.Api.Abstracoes.Infraestrutura;
using TorneSe.CapturaPagamento.Api.Common;
using TorneSe.CapturaPagamento.Api.Configuration;
using TorneSe.CapturaPagamento.Api.Domain.Constants;
using TorneSe.CapturaPagamento.Api.Domain.Entities;
using TorneSe.CapturaPagamento.Api.Domain.Enums;
using TorneSe.CapturaPagamento.Api.UseCases.ProcessarWebhookStripe.Request;
using TorneSe.CapturaPagamento.Api.UseCases.ProcessarWebhookStripe.Response;

namespace TorneSe.CapturaPagamento.Api.UseCases.ProcessarWebhookStripe;

/// <summary>
/// Handler responsável por processar eventos de webhook do Stripe e publicá-los na fila SQS.
/// </summary>
public sealed class Handler(
    ILogger<Handler> logger,
    IMapper mapper,
    IPaymentEventPublisher publisher,
    IOptions<StripeOptions> stripeOptions)
    : IRequestHandler<ProcessarWebhookStripeRequest, Result<ProcessarWebhookStripeResponse>>
{
    private readonly StripeOptions _stripeOptions = stripeOptions.Value;

    public async Task<Result<ProcessarWebhookStripeResponse>> Handle(
        ProcessarWebhookStripeRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validar entrada
            if (string.IsNullOrEmpty(request.Event.Id) || string.IsNullOrEmpty(request.Event.Type))
            {
                logger.LogWarning("Evento do Stripe inválido: faltam Id ou Type");
                return Result<ProcessarWebhookStripeResponse>.Error("Evento inválido");
            }

            // Validar assinatura em produção
            if (_stripeOptions.ValidateSignature && string.IsNullOrEmpty(request.SignatureHeader))
            {
                logger.LogWarning("Signature header ausente em ambiente que requer validação");
                return Result<ProcessarWebhookStripeResponse>.Error("Assinatura não fornecida");
            }

            logger.LogInformation(
                "Processando evento Stripe {EventId} do tipo {EventType}",
                request.Event.Id,
                request.Event.Type);

            // Mapear para entidade de domínio
            var paymentEvent = MapToDomain(request.Event);

            // Publicar na fila SQS
            var publicado = await publisher.PublishAsync(paymentEvent, cancellationToken);

            if (!publicado)
            {
                logger.LogError(
                    "Falha ao publicar evento {EventId} na fila SQS",
                    request.Event.Id);
                return Result<ProcessarWebhookStripeResponse>.Error("Erro ao publicar evento");
            }

            // Retornar resposta de sucesso
            var response = new ProcessarWebhookStripeResponse
            {
                EventId = paymentEvent.Id,
                EventType = paymentEvent.EventType.ToString(),
                Status = "Accepted",
                ProcessedAt = DateTime.UtcNow
            };

            logger.LogInformation(
                "Evento {EventId} processado e publicado com sucesso",
                request.Event.Id);

            return Result<ProcessarWebhookStripeResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Erro ao processar evento Stripe {EventId}",
                request.Event?.Id ?? "unknown");
            return Result<ProcessarWebhookStripeResponse>.Error("Erro ao processar webhook");
        }
    }

    private PaymentEvent MapToDomain(StripeEventDto dto)
    {
        var paymentEvent = new PaymentEvent
        {
            StripeEventId = dto.Id,
            EventType = ParseEventType(dto.Type),
            EventTimestamp = DateTimeOffset.FromUnixTimeSeconds(dto.Created).UtcDateTime,
            PayloadJson = JsonSerializer.Serialize(dto.Data.Object, AppConstants.JsonSerializerOptions),
            IsLiveMode = dto.Livemode
        };

        // Extrair informações específicas do objeto, se disponíveis
        if (dto.Data.Object.TryGetValue("customer", out var customer))
        {
            paymentEvent.CustomerId = customer?.ToString();
        }

        if (dto.Data.Object.TryGetValue("amount", out var amount))
        {
            if (long.TryParse(amount?.ToString(), out var amountCents))
            {
                paymentEvent.Amount = amountCents / 100m; // Converter centavos para valor decimal
            }
        }

        if (dto.Data.Object.TryGetValue("currency", out var currency))
        {
            paymentEvent.Currency = currency?.ToString()?.ToUpperInvariant();
        }

        if (dto.Data.Object.TryGetValue("payment_intent", out var paymentIntent))
        {
            paymentEvent.PaymentIntentId = paymentIntent?.ToString();
        }

        if (dto.Data.Object.TryGetValue("id", out var chargeId) && dto.Type.StartsWith("charge."))
        {
            paymentEvent.ChargeId = chargeId?.ToString();
        }

        return paymentEvent;
    }

    private static PaymentEventType ParseEventType(string stripeEventType)
    {
        return stripeEventType switch
        {
            "payment_intent.succeeded" => PaymentEventType.PaymentIntentSucceeded,
            "payment_intent.payment_failed" => PaymentEventType.PaymentIntentFailed,
            "charge.succeeded" => PaymentEventType.ChargeSucceeded,
            "charge.failed" => PaymentEventType.ChargeFailed,
            "charge.refunded" => PaymentEventType.ChargeRefunded,
            "payment_method.attached" => PaymentEventType.PaymentMethodAttached,
            "customer.created" => PaymentEventType.CustomerCreated,
            "customer.updated" => PaymentEventType.CustomerUpdated,
            _ => PaymentEventType.Unknown
        };
    }
}
