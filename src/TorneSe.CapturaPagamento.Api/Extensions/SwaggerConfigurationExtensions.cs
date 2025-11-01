using Microsoft.OpenApi.Models;

namespace TorneSe.CapturaPagamento.Api.Extensions;

/// <summary>
/// Extensions para configuração do Swagger/OpenAPI.
/// </summary>
public static class SwaggerConfigurationExtensions
{
    /// <summary>
    /// Adiciona e configura o Swagger na aplicação.
    /// </summary>
    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "TorneSe Captura Pagamento API",
                Version = "v1",
                Description = "API para captura e processamento de pagamentos"
            });
        });

        return services;
    }

    /// <summary>
    /// Configura a interface do Swagger na aplicação.
    /// </summary>
    public static WebApplication UseSwaggerInterface(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "TorneSe Captura Pagamento API v1");
            options.RoutePrefix = string.Empty; // Swagger na raiz
        });

        return app;
    }
}
