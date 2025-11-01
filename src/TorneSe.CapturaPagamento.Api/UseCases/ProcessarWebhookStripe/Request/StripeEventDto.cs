using System.Text.Json.Serialization;

namespace TorneSe.CapturaPagamento.Api.UseCases.ProcessarWebhookStripe.Request;

/// <summary>
/// DTO representando o payload recebido do webhook do Stripe.
/// </summary>
public class StripeEventDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("created")]
    public long Created { get; set; }

    [JsonPropertyName("data")]
    public StripeEventDataDto Data { get; set; } = new();

    [JsonPropertyName("livemode")]
    public bool Livemode { get; set; }
}

/// <summary>
/// Dados do evento Stripe.
/// </summary>
public class StripeEventDataDto
{
    [JsonPropertyName("object")]
    public Dictionary<string, object> Object { get; set; } = new();
}
