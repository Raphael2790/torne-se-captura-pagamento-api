using TorneSe.CapturaPagamento.Api.Domain.Enums;

namespace TorneSe.CapturaPagamento.Api.Domain.Entities;

/// <summary>
/// Entidade de domínio representando um evento de pagamento.
/// </summary>
public sealed class PaymentEvent : Entity
{
    public string StripeEventId { get; set; } = string.Empty;
    public PaymentEventType EventType { get; set; }
    public DateTime EventTimestamp { get; set; }
    public string PayloadJson { get; set; } = string.Empty;
    public bool IsLiveMode { get; set; }
    public string? CustomerId { get; set; }
    public decimal? Amount { get; set; }
    public string? Currency { get; set; }
    public string? PaymentIntentId { get; set; }
    public string? ChargeId { get; set; }
}
