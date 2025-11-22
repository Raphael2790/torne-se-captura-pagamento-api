using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TorneSe.CapturaPagamento.Api.Common;
using TorneSe.CapturaPagamento.Api.UseCases.ProcessarWebhookStripe.Request;
using TorneSe.CapturaPagamento.Api.UseCases.ProcessarWebhookStripe.Response;

namespace TorneSe.CapturaPagamento.Api.Controllers;

/// <summary>
/// Endpoints para webhooks de serviços externos
/// </summary>
public static class WebhooksApiEndpoints
{
    /// <summary>
    /// Mapeia os endpoints de webhooks
    /// </summary>
    public static void MapWebhooksEndpoints(this IEndpointRouteBuilder app)
    {
        var webhooksGroup = app.MapGroup("webhooks")
            .WithTags("Webhooks");

        // Endpoint para receber webhooks do Stripe
        webhooksGroup.MapPost("/stripe", async Task<Results<Accepted, BadRequest<Result<ProcessarWebhookStripeResponse>>>> (
            [FromServices] IMediator mediator,
            [FromServices] ILogger<Program> logger,
            [FromBody] StripeEventDto @event,
            HttpContext httpContext) =>
        {
            try
            {
                // Capturar cabeçalho de assinatura
                var signatureHeader = httpContext.Request.Headers["Stripe-Signature"].FirstOrDefault();

                logger.LogInformation(
                    "Recebido webhook Stripe - EventId: {EventId}, Type: {EventType}",
                    @event.Id,
                    @event.Type);

                // Criar comando e processar
                var request = new ProcessarWebhookStripeRequest
                {
                    Event = @event,
                    SignatureHeader = signatureHeader
                };

                var result = await mediator.Send(request);

                if (result.IsSuccess)
                {
                    return TypedResults.Accepted("/webhooks/stripe");
                }

                logger.LogWarning(
                    "Falha ao processar webhook Stripe: {Message}",
                    result.Message);

                return TypedResults.BadRequest(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro não tratado ao processar webhook Stripe");
                var errorResult = Result<ProcessarWebhookStripeResponse>.Error("Erro ao processar webhook");
                return TypedResults.BadRequest(errorResult);
            }
        })
        .Produces(StatusCodes.Status202Accepted)
        .Produces<Result<ProcessarWebhookStripeResponse>>(StatusCodes.Status400BadRequest)
        .WithName("ProcessarWebhookStripe")
        .WithSummary("Recebe eventos de webhook do Stripe")
        .WithDescription("Endpoint para processar eventos enviados pelo Stripe via webhook");
    }

    /// <summary>
    /// Mapeia o endpoint de health check
    /// </summary>
    public static void MapHealthEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/health", () =>
        {
            return Results.Ok(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                service = "TorneSe.CapturaPagamento.Api"
            });
        })
        .Produces(StatusCodes.Status200OK)
        .WithName("HealthCheck")
        .WithTags("Health")
        .WithSummary("Verifica o status da aplicação")
        .WithDescription("Endpoint de health check para monitoramento da aplicação")
        .ExcludeFromDescription();
    }
}
