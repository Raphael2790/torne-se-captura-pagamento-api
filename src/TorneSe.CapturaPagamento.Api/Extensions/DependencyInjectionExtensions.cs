using System.Reflection;

namespace TorneSe.CapturaPagamento.Api.Extensions;

/// <summary>
/// Extensions para configuração de injeção de dependência.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adiciona os serviços da aplicação no container de DI.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
        });

        services.AddControllers();
        services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

        // Registrar MediatR para CQRS e eventos de domínio
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        // Registrar AutoMapper para mapeamentos
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // Aqui você pode adicionar seus serviços customizados
        // services.TryAddScoped<IDbService, DbService>();
        // services.TryAddScoped<IMessageService, MessageService>();

        return services;
    }
}
