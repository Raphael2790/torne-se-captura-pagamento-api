using MediatR;

namespace TorneSe.CapturaPagamento.Api.Domain.Messages;

/// <summary>
/// Classe base abstrata para mensagens de eventos e notificações.
/// Implementa INotification do MediatR para suporte a eventos de domínio.
/// </summary>
public abstract class Message : INotification
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }

    protected Message()
    {
        Id = Guid.NewGuid();
        Timestamp = DateTime.Now;
    }
}
