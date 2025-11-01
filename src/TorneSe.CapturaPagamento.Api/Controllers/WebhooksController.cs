using MediatR;
using Microsoft.AspNetCore.Mvc;
using TorneSe.CapturaPagamento.Api.UseCases.ProcessarWebhookStripe.Request;

namespace TorneSe.CapturaPagamento.Api.Controllers;

/// <summary>
/// Controller responsável por receber webhooks de serviços externos.
/// </summary>
[ApiController]
[Route("webhooks")]
public class WebhooksController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<WebhooksController> _logger;

    public WebhooksController(IMediator mediator, ILogger<WebhooksController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Recebe eventos de webhook do Stripe.
    /// </summary>
    /// <param name="event">Payload do evento do Stripe</param>
    /// <returns>202 Accepted se processado com sucesso</returns>
    [HttpPost("stripe")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ReceberWebhookStripe([FromBody] StripeEventDto @event)
    {
        try
        {
            // Capturar cabeçalho de assinatura
            var signatureHeader = Request.Headers["Stripe-Signature"].FirstOrDefault();

            _logger.LogInformation(
                "Recebido webhook Stripe - EventId: {EventId}, Type: {EventType}",
                @event.Id,
                @event.Type);

            // Criar comando e processar
            var request = new ProcessarWebhookStripeRequest
            {
                Event = @event,
                SignatureHeader = signatureHeader
            };

            var result = await _mediator.Send(request);

            if (result.IsSuccess)
            {
                return Accepted(result);
            }

            _logger.LogWarning(
                "Falha ao processar webhook Stripe: {Message}",
                result.Message);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado ao processar webhook Stripe");
            return StatusCode(500, new { error = "Erro ao processar webhook" });
        }
    }

    /// <summary>
    /// Endpoint de health check.
    /// </summary>
    /// <returns>Status da aplicação</returns>
    [HttpGet("/health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow,
            service = "TorneSe.CapturaPagamento.Api"
        });
    }
}
