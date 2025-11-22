# Guia de Deploy - TorneSe Captura Pagamento API

## Deploy Automatizado com GitHub Actions

Este projeto possui um workflow de CI/CD configurado que realiza o deploy automaticamente na AWS Lambda quando há push para a branch `main`.

### Workflow: `.github/workflows/deploy.yml`

O workflow executa as seguintes etapas:

1. **Build and Test**
   - Checkout do código
   - Setup do .NET 8.0
   - Restore de dependências
   - Build do projeto
   - Execução de testes unitários
   - Publicação dos resultados dos testes

2. **Package and Deploy**
   - Empacotamento da aplicação
   - Upload do pacote para S3
   - Atualização do código da Lambda
   - Configuração das variáveis de ambiente
   - Publicação de nova versão

### GitHub Secrets Necessários

Configure os seguintes secrets no GitHub (Settings → Secrets and variables → Actions):

#### Credenciais AWS (Obrigatórios)

| Secret | Descrição | Exemplo |
|--------|-----------|---------|
| `AWS_ACCESS_KEY_ID` | Access Key ID da AWS | `AKIAIOSFODNN7EXAMPLE` |
| `AWS_SECRET_ACCESS_KEY` | Secret Access Key da AWS | `wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY` |

#### Variáveis de Ambiente da Lambda (Obrigatórios)

| Secret | Descrição | Exemplo |
|--------|-----------|---------|
| `SQS_QUEUE_URL` | URL completa da fila SQS | `https://sqs.us-east-1.amazonaws.com/123456789/payment-events` |
| `APP_ENV` | Ambiente de execução | `Production` |
| `LOG_LEVEL` | Nível de log | `Information` |
| `STRIPE_SIGNING_SECRET` | Secret do webhook do Stripe | `whsec_...` |

### Variáveis de Ambiente do Workflow

Estas variáveis estão definidas no arquivo `deploy.yml` e podem ser ajustadas conforme necessário:

| Variável | Descrição | Valor Padrão |
|----------|-----------|--------------|
| `AWS_REGION` | Região AWS | `us-east-1` |
| `DOTNET_VERSION` | Versão do .NET | `8.0.x` |
| `PROJECT_PATH` | Caminho do projeto | `src/TorneSe.CapturaPagamento.Api` |
| `S3_BUCKET` | Bucket S3 para deployments | `torne-se-captura-pagamento-api-deployments` |
| `LAMBDA_FUNCTION_NAME` | Nome da função Lambda | `TorneSe-CapturaPagamento-Api` |

### Como Fazer Deploy

#### Deploy Automático
1. Faça commit e push para a branch `main`
2. O workflow será executado automaticamente
3. Acompanhe o progresso em Actions → Deploy to AWS Lambda

#### Deploy Manual
1. Acesse Actions → Deploy to AWS Lambda
2. Clique em "Run workflow"
3. Selecione a branch e clique em "Run workflow"

### Pré-requisitos na AWS

Antes de executar o workflow, certifique-se de que os seguintes recursos existem na AWS:

1. **Função Lambda**
   - Nome: `TorneSe-CapturaPagamento-Api`
   - Runtime: .NET 8 (Amazon Linux 2)
   - Handler: `TorneSe.CapturaPagamento.Api`
   - IAM Role com permissões para:
     - Logs (CloudWatch)
     - SQS (SendMessage)

2. **Fila SQS**
   - Fila criada para receber eventos de pagamento
   - URL da fila configurada no secret `SQS_QUEUE_URL`

3. **API Gateway** (Opcional)
   - HTTP API configurado para invocar a Lambda
   - Rota: `POST /webhooks/stripe`

4. **Bucket S3** (Criado automaticamente)
   - Nome: `torne-se-captura-pagamento-api-deployments`
   - Criado automaticamente pelo workflow se não existir

## Scripts de Empacotamento Manual

Para deploy manual, utilize os scripts de empacotamento:

### Para Windows (PowerShell)

```powershell
.\deploy\package-windows.ps1
```

### Para Linux/Mac (Bash)

```bash
chmod +x ./deploy/package-linux.sh
./deploy/package-linux.sh
```

## Variáveis de Ambiente da Lambda

Configure as seguintes variáveis de ambiente na AWS Lambda (gerenciadas automaticamente pelo workflow):

### Obrigatórias

| Variável | Descrição | Exemplo |
|----------|-----------|---------|
| `AWS_REGION` | Região AWS para SQS | `us-east-1` |
| `SQS_QUEUE_URL` | URL completa da fila SQS | `https://sqs.us-east-1.amazonaws.com/123456789/payment-events` |
| `APP_ENV` | Ambiente de execução | `Production` |
| `LOG_LEVEL` | Nível de log | `Information` |
| `STRIPE_SIGNING_SECRET` | Secret do webhook do Stripe | `whsec_...` |

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
