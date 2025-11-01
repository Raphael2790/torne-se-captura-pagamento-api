using MediatR;
using TorneSe.CapturaPagamento.Api.Common;
using TorneSe.CapturaPagamento.Api.UseCases.ProcessarWebhookStripe.Response;

namespace TorneSe.CapturaPagamento.Api.UseCases.ProcessarWebhookStripe.Request;

/// <summary>
/// Command para processar e publicar evento de pagamento do Stripe.
/// </summary>
public class ProcessarWebhookStripeRequest : IRequest<Result<ProcessarWebhookStripeResponse>>
{
    public StripeEventDto Event { get; set; } = new();
    public string? SignatureHeader { get; set; }
}
