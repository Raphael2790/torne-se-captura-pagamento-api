using System.Net;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TorneSe.CapturaPagamento.Api.Configuration;
using TorneSe.CapturaPagamento.Api.Domain.Entities;
using TorneSe.CapturaPagamento.Api.Domain.Enums;
using TorneSe.CapturaPagamento.Api.Infraestrutura.Services;

namespace TorneSe.CapturaPagamento.Api.Tests.Infraestrutura.Services;

public class SqsPaymentEventPublisherTests
{
    private readonly Mock<ILogger<SqsPaymentEventPublisher>> _loggerMock;
    private readonly Mock<IAmazonSQS> _sqsClientMock;
    private readonly Mock<IOptions<AwsOptions>> _awsOptionsMock;
    private readonly AwsOptions _awsOptions;
    private readonly SqsPaymentEventPublisher _sut;

    public SqsPaymentEventPublisherTests()
    {
        _loggerMock = new Mock<ILogger<SqsPaymentEventPublisher>>();
        _sqsClientMock = new Mock<IAmazonSQS>();
        _awsOptionsMock = new Mock<IOptions<AwsOptions>>();

        _awsOptions = new AwsOptions
        {
            Region = "us-east-1",
            SqsQueueUrl = "https://sqs.us-east-1.amazonaws.com/123456789012/test-queue"
        };

        _awsOptionsMock.Setup(x => x.Value).Returns(_awsOptions);

        _sut = new SqsPaymentEventPublisher(
            _loggerMock.Object,
            _sqsClientMock.Object,
            _awsOptionsMock.Object);
    }

    private static PaymentEvent CriarPaymentEventValido()
    {
        return new PaymentEvent
        {
            Id = Guid.NewGuid(),
            StripeEventId = "evt_test_123",
            EventType = PaymentEventType.PaymentIntentSucceeded,
            EventTimestamp = DateTime.UtcNow,
            PayloadJson = "{}",
            IsLiveMode = false,
            CustomerId = "cus_test_123",
            Amount = 1000m,
            Currency = "brl",
            PaymentIntentId = "pi_test_123",
            ChargeId = "ch_test_123"
        };
    }

    [Fact]
    public async Task PublishAsync_QuandoEnvioComSucesso_EntaoRetornaTrue()
    {
        // Arrange
        var paymentEvent = CriarPaymentEventValido();
        var expectedResponse = new SendMessageResponse
        {
            HttpStatusCode = HttpStatusCode.OK,
            MessageId = Guid.NewGuid().ToString()
        };

        _sqsClientMock
            .Setup(x => x.SendMessageAsync(
                It.IsAny<SendMessageRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _sut.PublishAsync(paymentEvent);

        // Assert
        Assert.True(result);
        _sqsClientMock.Verify(
            x => x.SendMessageAsync(
                It.Is<SendMessageRequest>(req =>
                    req.QueueUrl == _awsOptions.SqsQueueUrl &&
                    req.MessageAttributes.ContainsKey("EventType") &&
                    req.MessageAttributes.ContainsKey("EventId") &&
                    req.MessageAttributes.ContainsKey("StripeEventId")),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task PublishAsync_QuandoEnvioComSucesso_EntaoAtributosDeMessageSaoConfiguradosCorretamente()
    {
        // Arrange
        var paymentEvent = CriarPaymentEventValido();
        SendMessageRequest? capturedRequest = null;

        _sqsClientMock
            .Setup(x => x.SendMessageAsync(
                It.IsAny<SendMessageRequest>(),
                It.IsAny<CancellationToken>()))
            .Callback<SendMessageRequest, CancellationToken>((req, _) => capturedRequest = req)
            .ReturnsAsync(new SendMessageResponse
            {
                HttpStatusCode = HttpStatusCode.OK,
                MessageId = Guid.NewGuid().ToString()
            });

        // Act
        await _sut.PublishAsync(paymentEvent);

        // Assert
        Assert.NotNull(capturedRequest);
        Assert.Equal(_awsOptions.SqsQueueUrl, capturedRequest.QueueUrl);
        Assert.Equal(paymentEvent.EventType.ToString(), capturedRequest.MessageAttributes["EventType"].StringValue);
        Assert.Equal(paymentEvent.Id.ToString(), capturedRequest.MessageAttributes["EventId"].StringValue);
        Assert.Equal(paymentEvent.StripeEventId, capturedRequest.MessageAttributes["StripeEventId"].StringValue);
    }

    [Fact]
    public async Task PublishAsync_QuandoHttpStatusCodeNaoEOk_EntaoRetornaFalse()
    {
        // Arrange
        var paymentEvent = CriarPaymentEventValido();
        var expectedResponse = new SendMessageResponse
        {
            HttpStatusCode = HttpStatusCode.InternalServerError,
            MessageId = string.Empty
        };

        _sqsClientMock
            .Setup(x => x.SendMessageAsync(
                It.IsAny<SendMessageRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _sut.PublishAsync(paymentEvent);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task PublishAsync_QuandoSqsClientLancaExcecao_EntaoRetornaFalse()
    {
        // Arrange
        var paymentEvent = CriarPaymentEventValido();

        _sqsClientMock
            .Setup(x => x.SendMessageAsync(
                It.IsAny<SendMessageRequest>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AmazonSQSException("Erro de conexão com SQS"));

        // Act
        var result = await _sut.PublishAsync(paymentEvent);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task PublishAsync_QuandoExcecaoGenerica_EntaoRetornaFalse()
    {
        // Arrange
        var paymentEvent = CriarPaymentEventValido();

        _sqsClientMock
            .Setup(x => x.SendMessageAsync(
                It.IsAny<SendMessageRequest>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Erro inesperado"));

        // Act
        var result = await _sut.PublishAsync(paymentEvent);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task PublishAsync_QuandoCancellationTokenECancelado_EntaoOperacaoCanceladaException()
    {
        // Arrange
        var paymentEvent = CriarPaymentEventValido();
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        _sqsClientMock
            .Setup(x => x.SendMessageAsync(
                It.IsAny<SendMessageRequest>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = await _sut.PublishAsync(paymentEvent, cancellationTokenSource.Token);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task PublishAsync_QuandoEnvioComSucesso_EntaoLogaInformacoesCorretas()
    {
        // Arrange
        var paymentEvent = CriarPaymentEventValido();
        var messageId = Guid.NewGuid().ToString();

        _sqsClientMock
            .Setup(x => x.SendMessageAsync(
                It.IsAny<SendMessageRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendMessageResponse
            {
                HttpStatusCode = HttpStatusCode.OK,
                MessageId = messageId
            });

        // Act
        await _sut.PublishAsync(paymentEvent);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((obj, type) => obj.ToString()!.Contains("Publicando evento de pagamento")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((obj, type) => obj.ToString()!.Contains("publicado com sucesso")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task PublishAsync_QuandoFalhaNoEnvio_EntaoLogaWarning()
    {
        // Arrange
        var paymentEvent = CriarPaymentEventValido();

        _sqsClientMock
            .Setup(x => x.SendMessageAsync(
                It.IsAny<SendMessageRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendMessageResponse
            {
                HttpStatusCode = HttpStatusCode.ServiceUnavailable,
                MessageId = string.Empty
            });

        // Act
        await _sut.PublishAsync(paymentEvent);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((obj, type) => obj.ToString()!.Contains("Falha ao publicar evento")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task PublishAsync_QuandoExcecao_EntaoLogaErro()
    {
        // Arrange
        var paymentEvent = CriarPaymentEventValido();
        var exception = new AmazonSQSException("Erro de conexão");

        _sqsClientMock
            .Setup(x => x.SendMessageAsync(
                It.IsAny<SendMessageRequest>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        await _sut.PublishAsync(paymentEvent);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((obj, type) => obj.ToString()!.Contains("Erro ao publicar evento")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(PaymentEventType.PaymentIntentSucceeded)]
    [InlineData(PaymentEventType.PaymentIntentFailed)]
    [InlineData(PaymentEventType.ChargeSucceeded)]
    [InlineData(PaymentEventType.ChargeFailed)]
    [InlineData(PaymentEventType.ChargeRefunded)]
    public async Task PublishAsync_QuandoDiferentesTiposDeEvento_EntaoEnviaComTipoCorreto(PaymentEventType eventType)
    {
        // Arrange
        var paymentEvent = CriarPaymentEventValido();
        paymentEvent.EventType = eventType;
        SendMessageRequest? capturedRequest = null;

        _sqsClientMock
            .Setup(x => x.SendMessageAsync(
                It.IsAny<SendMessageRequest>(),
                It.IsAny<CancellationToken>()))
            .Callback<SendMessageRequest, CancellationToken>((req, _) => capturedRequest = req)
            .ReturnsAsync(new SendMessageResponse
            {
                HttpStatusCode = HttpStatusCode.OK,
                MessageId = Guid.NewGuid().ToString()
            });

        // Act
        await _sut.PublishAsync(paymentEvent);

        // Assert
        Assert.NotNull(capturedRequest);
        Assert.Equal(eventType.ToString(), capturedRequest.MessageAttributes["EventType"].StringValue);
    }

    [Fact]
    public async Task PublishAsync_QuandoRepassaCancellationToken_EntaoTokenEUtilizado()
    {
        // Arrange
        var paymentEvent = CriarPaymentEventValido();
        var cancellationToken = new CancellationToken();

        _sqsClientMock
            .Setup(x => x.SendMessageAsync(
                It.IsAny<SendMessageRequest>(),
                cancellationToken))
            .ReturnsAsync(new SendMessageResponse
            {
                HttpStatusCode = HttpStatusCode.OK,
                MessageId = Guid.NewGuid().ToString()
            });

        // Act
        await _sut.PublishAsync(paymentEvent, cancellationToken);

        // Assert
        _sqsClientMock.Verify(
            x => x.SendMessageAsync(
                It.IsAny<SendMessageRequest>(),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task PublishAsync_QuandoMessageBodySerializado_EntaoContemDadosDoEvento()
    {
        // Arrange
        var paymentEvent = CriarPaymentEventValido();
        SendMessageRequest? capturedRequest = null;

        _sqsClientMock
            .Setup(x => x.SendMessageAsync(
                It.IsAny<SendMessageRequest>(),
                It.IsAny<CancellationToken>()))
            .Callback<SendMessageRequest, CancellationToken>((req, _) => capturedRequest = req)
            .ReturnsAsync(new SendMessageResponse
            {
                HttpStatusCode = HttpStatusCode.OK,
                MessageId = Guid.NewGuid().ToString()
            });

        // Act
        await _sut.PublishAsync(paymentEvent);

        // Assert
        Assert.NotNull(capturedRequest);
        Assert.NotEmpty(capturedRequest.MessageBody);
        Assert.Contains(paymentEvent.StripeEventId, capturedRequest.MessageBody);
    }
}
