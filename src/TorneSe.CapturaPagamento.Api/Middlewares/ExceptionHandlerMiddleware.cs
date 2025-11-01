using System.Net;
using System.Text.Json;

namespace TorneSe.CapturaPagamento.Api.Middlewares;

/// <summary>
/// Middleware para tratamento global de exceções não tratadas.
/// Captura exceções e retorna uma resposta JSON padronizada.
/// </summary>
public class ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger)
    : IMiddleware
{
    private readonly ILogger<ExceptionHandlerMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new
        {
            context.Response.StatusCode,
            Message = "Ocorreu um erro durante o processamento da requisição.",
            TraceId = context.TraceIdentifier
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }
}
