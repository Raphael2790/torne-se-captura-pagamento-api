namespace TorneSe.CapturaPagamento.Api.Domain.Entities;

/// <summary>
/// Classe base abstrata para todas as entidades do domínio.
/// Fornece propriedades comuns como Id e DataCriacao.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; set; }
    public DateTime DataCriacao { get; set; }

    protected Entity()
    {
        Id = Guid.NewGuid();
        DataCriacao = DateTime.Now;
    }
}
