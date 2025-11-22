# Request Tests - TorneSe Captura Pagamento API

Esta pasta contém arquivos `.http` para testar os endpoints da API localmente usando a extensão REST Client do VS Code.

## Pré-requisitos

- [REST Client Extension](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) instalada no VS Code
- Aplicação rodando localmente em `http://localhost:5000`

## Arquivos Disponíveis

### stripe-payment_succeeded.http
Testa o evento `payment_intent.succeeded` do Stripe
- Simula um pagamento bem-sucedido
- Valor: R$ 50,00 (5000 centavos)
- Cliente: `cus_test_12345`

### stripe-charge_refunded.http
Testa o evento `charge.refunded` do Stripe
- Simula um reembolso total
- Valor reembolsado: R$ 50,00
- Status: `refunded`

### stripe-generic.http
Contém múltiplos cenários de teste:
1. **Health Check** - Verifica se a API está funcionando
2. **Customer Created** - Evento de criação de cliente
3. **Charge Succeeded** - Cobrança bem-sucedida
4. **Payment Intent Failed** - Tentativa de pagamento que falhou

## Como Usar

1. Certifique-se de que a aplicação está rodando:
   ```bash
   dotnet run --project src/TorneSe.CapturaPagamento.Api
   ```

2. Abra qualquer arquivo `.http` no VS Code

3. Clique em "Send Request" acima de cada requisição OU use o atalho `Ctrl+Alt+R`

4. Veja a resposta diretamente no VS Code

## Variáveis de Ambiente

Os arquivos usam as seguintes variáveis:
- `@host` - URL base da API (padrão: `http://localhost:5000`)
- `@stripeSignature` - Assinatura do webhook (para testes: `whsec_test_signature_12345`)

### Modo Desenvolvimento
Por padrão, a validação de assinatura está **desabilitada** em ambiente de desenvolvimento.
Veja `appsettings.Development.json`:
```json
{
  "Stripe": {
    "ValidateSignature": false
  }
}
```

### Modo Produção
Em produção, configure as variáveis de ambiente:
```bash
Stripe__SigningSecret=whsec_seu_secret_real
Stripe__ValidateSignature=true
```

## Exemplos de Respostas

### Sucesso (202 Accepted)
```json
{
  "isSuccess": true,
  "message": null,
  "data": {
    "eventId": "guid-aqui",
    "eventType": "PaymentIntentSucceeded",
    "status": "Accepted",
    "processedAt": "2024-01-01T00:00:00Z"
  }
}
```

### Erro (400 Bad Request)
```json
{
  "isSuccess": false,
  "message": "Evento do tipo 'unknown.event' não é suportado",
  "data": null
}
```

## Testando com Stripe CLI

Para testar com eventos reais do Stripe:

```bash
# Instalar Stripe CLI
# https://stripe.com/docs/stripe-cli

# Autenticar
stripe login

# Escutar webhooks e encaminhar para API local
stripe listen --forward-to http://localhost:5000/webhooks/stripe

# Em outro terminal, disparar evento de teste
stripe trigger payment_intent.succeeded
```

## Estrutura dos Eventos Stripe

Todos os eventos seguem o formato:
```json
{
  "id": "evt_...",
  "object": "event",
  "type": "payment_intent.succeeded",
  "created": 1678912345,
  "livemode": false,
  "data": {
    "object": {
      // Dados específicos do evento
    }
  }
}
```

## Tipos de Eventos Suportados

1. `payment_intent.succeeded` - Pagamento confirmado
2. `payment_intent.payment_failed` - Pagamento falhou
3. `charge.succeeded` - Cobrança bem-sucedida
4. `charge.failed` - Cobrança falhou
5. `charge.refunded` - Reembolso processado
6. `payment_method.attached` - Método de pagamento anexado
7. `customer.created` - Cliente criado
8. `customer.updated` - Cliente atualizado

## Troubleshooting

### Erro de conexão
- Verifique se a aplicação está rodando
- Confirme a porta correta (padrão: 5000)

### Erro 400 - Bad Request
- Verifique o formato JSON do payload
- Confirme que o tipo de evento é suportado

### Logs não aparecem
- Verifique o console onde a aplicação está rodando
- Logs estruturados aparecem com `[INF]` ou `[ERR]`

## Monitoramento

Após enviar requisições, verifique:
1. **Console da aplicação** - Logs em tempo real
2. **AWS CloudWatch** (em produção) - Logs persistidos
3. **AWS SQS Console** - Mensagens na fila

## Dicas

- Use `###` para separar múltiplas requisições no mesmo arquivo
- Você pode ter múltiplas definições de variáveis `@host` para diferentes ambientes
- A extensão REST Client salva o histórico de requisições
- Use `Ctrl+P` e digite `>Rest Client` para ver todos os comandos disponíveis
