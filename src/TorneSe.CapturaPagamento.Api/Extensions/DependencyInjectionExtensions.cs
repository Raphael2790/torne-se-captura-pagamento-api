using System.Reflection;
using Amazon.SQS;
using TorneSe.CapturaPagamento.Api.Abstracoes.Infraestrutura;
using TorneSe.CapturaPagamento.Api.Configuration;
using TorneSe.CapturaPagamento.Api.Infraestrutura.Services;

namespace TorneSe.CapturaPagamento.Api.Extensions;

/// <summary>
/// Extensions para configuração de injeção de dependência.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adiciona os serviços da aplicação no container de DI.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
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

        // Configurar Options Pattern
        services.Configure<AwsOptions>(configuration.GetSection("Aws"));
        services.Configure<StripeOptions>(configuration.GetSection("Stripe"));

        // Registrar AWS SQS
        services.AddAWSService<IAmazonSQS>();

        // Registrar serviços de infraestrutura
        services.AddScoped<IPaymentEventPublisher, SqsPaymentEventPublisher>();

        return services;
    }
}
