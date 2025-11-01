# Guia de Deploy - TorneSe Captura Pagamento API

## Scripts de Empacotamento

Este diretório contém scripts para empacotar a aplicação para deploy na AWS Lambda.

### Para Windows (PowerShell)

```powershell
.\deploy\package-windows.ps1
```

### Para Linux/Mac (Bash)

```bash
chmod +x ./deploy/package-linux.sh
./deploy/package-linux.sh
```

## Variáveis de Ambiente Necessárias

Configure as seguintes variáveis de ambiente na AWS Lambda:

### Obrigatórias

| Variável | Descrição | Exemplo |
|----------|-----------|---------|
| `AWS_REGION` | Região AWS para SQS | `us-east-1` |
| `Aws__SqsQueueUrl` | URL completa da fila SQS | `https://sqs.us-east-1.amazonaws.com/123456789/payment-events` |
| `Stripe__SigningSecret` | Secret do webhook do Stripe | `whsec_...` |

### Opcionais

| Variável | Descrição | Padrão |
|----------|-----------|--------|
| `AppEnvironment` | Ambiente de execução | `Production` |
| `LogLevel` | Nível de log | `Information` |
| `Stripe__ValidateSignature` | Validar assinatura Stripe | `true` |

## Estrutura da Lambda

### Runtime
- **.NET 8.0** (Amazon Linux 2)

### Handler
```
TorneSe.CapturaPagamento.Api::TorneSe.CapturaPagamento.Api.LambdaEntryPoint::FunctionHandlerAsync
```

### Timeout Recomendado
- **30 segundos**

### Memória Recomendada
- **512 MB** (ajuste conforme necessidade)

## Permissões IAM Necessárias

A função Lambda precisa das seguintes permissões:

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "sqs:SendMessage",
        "sqs:GetQueueUrl"
      ],
      "Resource": "arn:aws:sqs:us-east-1:123456789:payment-events"
    },
    {
      "Effect": "Allow",
      "Action": [
        "logs:CreateLogGroup",
        "logs:CreateLogStream",
        "logs:PutLogEvents"
      ],
      "Resource": "arn:aws:logs:*:*:*"
    }
  ]
}
```

## Integração com API Gateway

### HTTP API (Recomendado)
1. Crie uma HTTP API no API Gateway
2. Configure integração com a Lambda
3. Configure rota `POST /webhooks/stripe`
4. Configure rota `GET /health`

### Configurações Importantes
- **Payload Format Version**: 2.0
- **Timeout**: 29 segundos (menor que o timeout da Lambda)

## Configurando Webhook no Stripe

1. Acesse o [Dashboard do Stripe](https://dashboard.stripe.com/webhooks)
2. Clique em "Add endpoint"
3. URL do endpoint: `https://sua-api.execute-api.us-east-1.amazonaws.com/webhooks/stripe`
4. Selecione os eventos:
   - `payment_intent.succeeded`
   - `payment_intent.payment_failed`
   - `charge.succeeded`
   - `charge.failed`
   - `charge.refunded`
   - `payment_method.attached`
   - `customer.created`
   - `customer.updated`
5. Copie o Signing Secret e configure na variável `Stripe__SigningSecret`

## Testando o Deploy

### Health Check
```bash
curl https://sua-api.execute-api.us-east-1.amazonaws.com/health
```

Resposta esperada:
```json
{
  "status": "Healthy",
  "timestamp": "2024-01-01T00:00:00Z",
  "service": "TorneSe.CapturaPagamento.Api"
}
```

### Teste de Webhook (usando Stripe CLI)
```bash
stripe listen --forward-to https://sua-api.execute-api.us-east-1.amazonaws.com/webhooks/stripe
stripe trigger payment_intent.succeeded
```

## Monitoramento

### CloudWatch Logs
Os logs da aplicação são enviados automaticamente para CloudWatch Logs.

Log Groups:
- `/aws/lambda/torne-se-captura-pagamento`

### Métricas Importantes
- Invocações
- Erros
- Duração
- Throttles
- Mensagens SQS enviadas

## Troubleshooting

### Erro: "SignatureVerificationFailed"
- Verifique se o `Stripe__SigningSecret` está correto
- Em desenvolvimento, configure `Stripe__ValidateSignature=false`

### Erro: "AccessDenied" ao enviar para SQS
- Verifique as permissões IAM da Lambda
- Confirme que a URL da fila SQS está correta

### Erro: Timeout
- Aumente o timeout da Lambda (máximo 15 minutos)
- Verifique se há problemas de rede com a fila SQS

## Comandos Úteis

### Testar localmente (requer SAM CLI)
```bash
sam local start-api
```

### Ver logs da Lambda
```bash
aws logs tail /aws/lambda/torne-se-captura-pagamento --follow
```

### Listar mensagens na fila SQS
```bash
aws sqs receive-message --queue-url https://sqs.us-east-1.amazonaws.com/123456789/payment-events
```

## Próximos Passos

Após o deploy:
1. Configure alertas no CloudWatch para erros
2. Configure dashboards para monitoramento
3. Implemente dead-letter queue (DLQ) para mensagens com falha
4. Configure X-Ray para tracing distribuído
5. Implemente CI/CD com GitHub Actions ou AWS CodePipeline
