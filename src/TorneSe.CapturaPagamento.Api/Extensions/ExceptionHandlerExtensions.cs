using TorneSe.CapturaPagamento.Api.Middlewares;

namespace TorneSe.CapturaPagamento.Api.Extensions;

/// <summary>
/// Extensions para configuração do middleware de tratamento de exceções.
/// </summary>
public static class ExceptionHandlerExtensions
{
    /// <summary>
    /// Registra o middleware de tratamento de exceções na coleção de serviços.
    /// </summary>
    public static IServiceCollection AddGlobalExceptionHandler(this IServiceCollection services)
    {
        services.AddTransient<ExceptionHandlerMiddleware>();
        return services;
    }

    /// <summary>
    /// Configura o middleware de tratamento de exceções na pipeline da aplicação.
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlerMiddleware>();
        return app;
    }
}
