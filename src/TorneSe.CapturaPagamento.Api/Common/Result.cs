namespace TorneSe.CapturaPagamento.Api.Common;

/// <summary>
/// Classe que representa o resultado de uma operação.
/// Implementa o pattern Result para tratamento de sucesso/falha de forma explícita.
/// </summary>
/// <typeparam name="T">Tipo do dado retornado em caso de sucesso</typeparam>
public class Result<T>
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    /// <summary>
    /// Cria um resultado de sucesso com os dados fornecidos.
    /// </summary>
    public static Result<T> Success(T data)
    {
        return new Result<T> { IsSuccess = true, Data = data };
    }

    /// <summary>
    /// Cria um resultado de erro com a mensagem fornecida.
    /// </summary>
    public static Result<T> Error(string message)
    {
        return new Result<T> { IsSuccess = false, Message = message };
    }
}
