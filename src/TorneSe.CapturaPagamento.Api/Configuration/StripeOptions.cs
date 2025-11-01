namespace TorneSe.CapturaPagamento.Api.Configuration;

/// <summary>
/// Configurações para validação de webhooks do Stripe.
/// </summary>
public class StripeOptions
{
    public string SigningSecret { get; set; } = string.Empty;
    public bool ValidateSignature { get; set; } = true;
}
