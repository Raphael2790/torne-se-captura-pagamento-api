namespace TorneSe.CapturaPagamento.Api.Configuration;

/// <summary>
/// Configurações para integração com AWS SQS.
/// </summary>
public class AwsOptions
{
    public string Region { get; set; } = "us-east-1";
    public string SqsQueueUrl { get; set; } = string.Empty;
}
