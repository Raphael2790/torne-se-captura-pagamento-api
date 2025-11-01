using TorneSe.CapturaPagamento.Api.Domain.Entities;

namespace TorneSe.CapturaPagamento.Api.Abstracoes.Infraestrutura;

/// <summary>
/// Interface para publicação de eventos de pagamento.
/// </summary>
public interface IPaymentEventPublisher
{
    /// <summary>
    /// Publica um evento de pagamento para processamento assíncrono.
    /// </summary>
    /// <param name="paymentEvent">Evento de pagamento a ser publicado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se publicado com sucesso, False caso contrário</returns>
    Task<bool> PublishAsync(PaymentEvent paymentEvent, CancellationToken cancellationToken = default);
}
