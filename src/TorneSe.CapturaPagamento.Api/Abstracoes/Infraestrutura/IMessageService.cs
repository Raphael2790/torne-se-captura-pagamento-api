using TorneSe.CapturaPagamento.Api.Domain.Messages;

namespace TorneSe.CapturaPagamento.Api.Abstracoes.Infraestrutura;

/// <summary>
/// Interface para serviço de mensageria.
/// Define operações de envio de mensagens para filas/tópicos.
/// </summary>
public interface IMessageService
{
    /// <summary>
    /// Envia uma mensagem para uma fila específica.
    /// </summary>
    /// <typeparam name="T">Tipo da mensagem (deve herdar de Message)</typeparam>
    /// <param name="message">Mensagem a ser enviada</param>
    /// <param name="queueUrl">URL da fila de destino</param>
    /// <returns>True se enviou com sucesso, False caso contrário</returns>
    Task<bool> SendAsync<T>(T message, string queueUrl) where T : Message;
}
