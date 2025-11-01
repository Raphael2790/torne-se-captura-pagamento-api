namespace TorneSe.CapturaPagamento.Api.Extensions;

/// <summary>
/// Extensions para configuração do pipeline da aplicação.
/// </summary>
public static class ConfigureAppExtensions
{
    /// <summary>
    /// Configura o pipeline de requisições da aplicação.
    /// </summary>
    public static WebApplication ConfigureApp(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}
