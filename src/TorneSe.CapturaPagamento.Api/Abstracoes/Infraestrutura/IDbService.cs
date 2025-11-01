namespace TorneSe.CapturaPagamento.Api.Abstracoes.Infraestrutura;

/// <summary>
/// Interface para serviço de banco de dados.
/// Define operações básicas de persistência.
/// </summary>
public interface IDbService
{
    /// <summary>
    /// Salva uma entidade no banco de dados.
    /// </summary>
    /// <typeparam name="T">Tipo da entidade</typeparam>
    /// <param name="entity">Entidade a ser salva</param>
    /// <returns>True se salvou com sucesso, False caso contrário</returns>
    Task<bool> SaveAsync<T>(T entity);
}
