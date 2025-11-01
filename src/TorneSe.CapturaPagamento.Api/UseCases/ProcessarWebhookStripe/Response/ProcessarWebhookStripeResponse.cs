namespace TorneSe.CapturaPagamento.Api.UseCases.ProcessarWebhookStripe.Response;

/// <summary>
/// Resposta do processamento do webhook do Stripe.
/// </summary>
public class ProcessarWebhookStripeResponse
{
    public Guid EventId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Status { get; set; } = "Accepted";
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}
