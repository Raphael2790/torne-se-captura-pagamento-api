using System.Text.Json;
using System.Text.Json.Serialization;

namespace TorneSe.CapturaPagamento.Api.Domain.Constants;

/// <summary>
/// Constantes globais da aplicação.
/// </summary>
public static class AppConstants
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    /// <summary>
    /// Opções padrão de serialização JSON para toda a aplicação.
    /// </summary>
    public static JsonSerializerOptions JsonSerializerOptions => _jsonSerializerOptions;
}
