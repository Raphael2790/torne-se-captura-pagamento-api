using System.Net;
using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;
using TorneSe.CapturaPagamento.Api.Abstracoes.Infraestrutura;
using TorneSe.CapturaPagamento.Api.Configuration;
using TorneSe.CapturaPagamento.Api.Domain.Constants;
using TorneSe.CapturaPagamento.Api.Domain.Entities;

namespace TorneSe.CapturaPagamento.Api.Infraestrutura.Services;

/// <summary>
/// Implementação do publisher de eventos de pagamento usando AWS SQS.
/// </summary>
public sealed class SqsPaymentEventPublisher(
    ILogger<SqsPaymentEventPublisher> logger,
    IAmazonSQS sqsClient,
    IOptions<AwsOptions> awsOptions) : IPaymentEventPublisher
{
    private readonly AwsOptions _awsOptions = awsOptions.Value;

    public async Task<bool> PublishAsync(PaymentEvent paymentEvent, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation(
                "Publicando evento de pagamento {EventId} do tipo {EventType} para SQS",
                paymentEvent.Id,
                paymentEvent.EventType);

            var messageBody = JsonSerializer.Serialize(paymentEvent, AppConstants.JsonSerializerOptions);

            var request = new SendMessageRequest
            {
                QueueUrl = _awsOptions.SqsQueueUrl,
                MessageBody = messageBody,
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
                {
                    {
                        "EventType",
                        new MessageAttributeValue
                        {
                            DataType = "String",
                            StringValue = paymentEvent.EventType.ToString()
                        }
                    },
                    {
                        "EventId",
                        new MessageAttributeValue
                        {
                            DataType = "String",
                            StringValue = paymentEvent.Id.ToString()
                        }
                    },
                    {
                        "StripeEventId",
                        new MessageAttributeValue
                        {
                            DataType = "String",
                            StringValue = paymentEvent.StripeEventId
                        }
                    }
                }
            };

            var response = await sqsClient.SendMessageAsync(request, cancellationToken);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                logger.LogInformation(
                    "Evento {EventId} publicado com sucesso. MessageId: {MessageId}",
                    paymentEvent.Id,
                    response.MessageId);
                return true;
            }

            logger.LogWarning(
                "Falha ao publicar evento {EventId}. StatusCode: {StatusCode}",
                paymentEvent.Id,
                response.HttpStatusCode);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Erro ao publicar evento {EventId} para SQS",
                paymentEvent.Id);
            return false;
        }
    }
}
