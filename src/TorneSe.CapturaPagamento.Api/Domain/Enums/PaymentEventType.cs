namespace TorneSe.CapturaPagamento.Api.Domain.Enums;

/// <summary>
/// Tipos de eventos de pagamento do Stripe.
/// </summary>
public enum PaymentEventType
{
    Unknown = 0,
    PaymentIntentSucceeded = 1,
    PaymentIntentFailed = 2,
    ChargeSucceeded = 3,
    ChargeFailed = 4,
    ChargeRefunded = 5,
    PaymentMethodAttached = 6,
    CustomerCreated = 7,
    CustomerUpdated = 8
}
