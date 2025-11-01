# Guia de Uso - TorneSe.CapturaPagamento.Api

## Como Usar a Nova Estrutura

Este guia demonstra como adicionar funcionalidades seguindo a arquitetura implementada.

---

## 1. Criando uma Nova Entidade

```csharp
// Domain/Entities/Pagamento.cs
namespace TorneSe.CapturaPagamento.Api.Domain.Entities;

public sealed class Pagamento : Entity
{
    public decimal Valor { get; set; }
    public string NumeroPedido { get; set; }
    public StatusPagamento Status { get; set; }
    public DateTime DataPagamento { get; set; }
    public string MetodoPagamento { get; set; }
}
```

---

## 2. Criando um Enum

```csharp
// Domain/Enums/StatusPagamento.cs
namespace TorneSe.CapturaPagamento.Api.Domain.Enums;

public enum StatusPagamento
{
    Pendente = 1,
    Processando = 2,
    Aprovado = 3,
    Recusado = 4,
    Cancelado = 5
}
```

---

## 3. Criando um Use Case (CQRS)

### Request (Command)

```csharp
// UseCases/ProcessarPagamento/Request/ProcessarPagamentoRequest.cs
using MediatR;
using TorneSe.CapturaPagamento.Api.Common;
using TorneSe.CapturaPagamento.Api.UseCases.ProcessarPagamento.Response;

namespace TorneSe.CapturaPagamento.Api.UseCases.ProcessarPagamento.Request;

public class ProcessarPagamentoRequest : IRequest<Result<ProcessarPagamentoResponse>>
{
    public string NumeroPedido { get; set; }
    public decimal Valor { get; set; }
    public string MetodoPagamento { get; set; }
    public string NumeroCartao { get; set; }
}
```

### Response

```csharp
// UseCases/ProcessarPagamento/Response/ProcessarPagamentoResponse.cs
using TorneSe.CapturaPagamento.Api.Domain.Enums;

namespace TorneSe.CapturaPagamento.Api.UseCases.ProcessarPagamento.Response;

public class ProcessarPagamentoResponse
{
    public Guid IdPagamento { get; set; }
    public StatusPagamento Status { get; set; }
    public string Mensagem { get; set; }
    public DateTime DataProcessamento { get; set; }
}
```

### Handler

```csharp
// UseCases/ProcessarPagamento/Handler.cs
using AutoMapper;
using MediatR;
using TorneSe.CapturaPagamento.Api.Abstracoes.Infraestrutura;
using TorneSe.CapturaPagamento.Api.Common;
using TorneSe.CapturaPagamento.Api.Domain.Entities;
using TorneSe.CapturaPagamento.Api.Domain.Enums;
using TorneSe.CapturaPagamento.Api.UseCases.ProcessarPagamento.Request;
using TorneSe.CapturaPagamento.Api.UseCases.ProcessarPagamento.Response;

namespace TorneSe.CapturaPagamento.Api.UseCases.ProcessarPagamento;

public sealed class Handler(
    ILogger<Handler> logger,
    IMapper mapper,
    IDbService dbService,
    IMediator mediator) 
    : IRequestHandler<ProcessarPagamentoRequest, Result<ProcessarPagamentoResponse>>
{
    public async Task<Result<ProcessarPagamentoResponse>> Handle(
        ProcessarPagamentoRequest request, 
        CancellationToken cancellationToken)
    {
        try
        {
            // Validações de negócio
            if (request.Valor <= 0)
                return Result<ProcessarPagamentoResponse>.Error("Valor inválido");

            // Criar entidade
            var pagamento = new Pagamento
            {
                NumeroPedido = request.NumeroPedido,
                Valor = request.Valor,
                MetodoPagamento = request.MetodoPagamento,
                Status = StatusPagamento.Processando,
                DataPagamento = DateTime.UtcNow
            };

            // Salvar no banco
            var salvou = await dbService.SaveAsync(pagamento);
            
            if (!salvou)
                return Result<ProcessarPagamentoResponse>.Error("Erro ao salvar pagamento");

            // Publicar evento (se necessário)
            // await mediator.Publish(new PagamentoCriadoMessage { ... }, cancellationToken);

            // Retornar resposta
            var response = new ProcessarPagamentoResponse
            {
                IdPagamento = pagamento.Id,
                Status = pagamento.Status,
                Mensagem = "Pagamento processado com sucesso",
                DataProcessamento = pagamento.DataPagamento
            };

            return Result<ProcessarPagamentoResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao processar pagamento");
            return Result<ProcessarPagamentoResponse>.Error("Erro ao processar pagamento");
        }
    }
}
```

---

## 4. Criando um Controller

```csharp
// Controllers/PagamentosController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TorneSe.CapturaPagamento.Api.UseCases.ProcessarPagamento.Request;

namespace TorneSe.CapturaPagamento.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PagamentosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PagamentosController> _logger;

    public PagamentosController(IMediator mediator, ILogger<PagamentosController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Processa um novo pagamento
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> ProcessarPagamento(
        [FromBody] ProcessarPagamentoRequest request)
    {
        _logger.LogInformation("Processando pagamento para pedido {NumeroPedido}", 
            request.NumeroPedido);

        var result = await _mediator.Send(request);

        if (result.IsSuccess)
            return Ok(result);

        return BadRequest(result);
    }
}
```

---

## 5. Criando uma Mensagem de Evento

```csharp
// Domain/Messages/PagamentoProcessadoMessage.cs
using TorneSe.CapturaPagamento.Api.Domain.Enums;

namespace TorneSe.CapturaPagamento.Api.Domain.Messages;

public sealed class PagamentoProcessadoMessage : Message
{
    public Guid IdPagamento { get; set; }
    public string NumeroPedido { get; set; }
    public StatusPagamento Status { get; set; }
    public decimal Valor { get; set; }
}
```

---

## 6. Criando um Event Handler

```csharp
// Handlers/PagamentoProcessado/NotificarClienteHandler.cs
using MediatR;
using TorneSe.CapturaPagamento.Api.Domain.Messages;

namespace TorneSe.CapturaPagamento.Api.Handlers.PagamentoProcessado;

public class NotificarClienteHandler(ILogger<NotificarClienteHandler> logger) 
    : INotificationHandler<PagamentoProcessadoMessage>
{
    public async Task Handle(
        PagamentoProcessadoMessage notification, 
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Notificando cliente sobre pagamento {IdPagamento}", 
            notification.IdPagamento);

        // Lógica para enviar notificação
        // Ex: Email, SMS, Push Notification
        
        await Task.CompletedTask;
    }
}
```

---

## 7. Criando Mapeamentos (AutoMapper)

```csharp
// Mappings/AutoMapperProfile.cs
using AutoMapper;
using TorneSe.CapturaPagamento.Api.Domain.Entities;
using TorneSe.CapturaPagamento.Api.UseCases.ProcessarPagamento.Request;

namespace TorneSe.CapturaPagamento.Api.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Mapeamento de Request para Entity
        CreateMap<ProcessarPagamentoRequest, Pagamento>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore());

        // Outros mapeamentos...
    }
}
```

---

## 8. Implementando um Service

```csharp
// Infraestrutura/Services/PagamentoService.cs
using TorneSe.CapturaPagamento.Api.Abstracoes.Infraestrutura;

namespace TorneSe.CapturaPagamento.Api.Infraestrutura.Services;

public sealed class PagamentoService(ILogger<PagamentoService> logger) : IDbService
{
    public async Task<bool> SaveAsync<T>(T entity)
    {
        try
        {
            // Implementação de salvamento
            // Ex: DynamoDB, SQL Server, etc.
            
            logger.LogInformation("Salvando entidade {EntityType}", typeof(T).Name);
            
            await Task.CompletedTask;
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao salvar entidade");
            return false;
        }
    }
}
```

---

## 9. Registrando Services no DI

```csharp
// Extensions/DependencyInjectionExtensions.cs
using Microsoft.Extensions.DependencyInjection.Extensions;
using TorneSe.CapturaPagamento.Api.Abstracoes.Infraestrutura;
using TorneSe.CapturaPagamento.Api.Infraestrutura.Services;

public static IServiceCollection AddApplicationServices(this IServiceCollection services)
{
    // ... código existente ...

    // Registrar seus serviços
    services.TryAddScoped<IDbService, PagamentoService>();
    
    return services;
}
```

---

## 10. Criando Configuration Options

```csharp
// Configuration/PagamentoOptions.cs
namespace TorneSe.CapturaPagamento.Api.Configuration;

public class PagamentoOptions
{
    public string GatewayUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public int TimeoutSegundos { get; set; } = 30;
}
```

### Registrar no Program.cs

```csharp
// Program.cs
using TorneSe.CapturaPagamento.Api.Configuration;

builder.Services.Configure<PagamentoOptions>(
    builder.Configuration.GetSection("Pagamento"));
```

### Usar no Handler

```csharp
public sealed class Handler(
    IOptions<PagamentoOptions> options) 
    : IRequestHandler<...>
{
    private readonly PagamentoOptions _options = options.Value;
    
    // Usar _options.GatewayUrl, etc.
}
```

---

## 11. Testando o Endpoint

### Usando Swagger
1. Acesse `http://localhost:5000` (Swagger na raiz)
2. Localize o endpoint desejado
3. Clique em "Try it out"
4. Preencha o body
5. Execute

### Usando cURL
```bash
curl -X POST "http://localhost:5000/api/pagamentos" \
  -H "Content-Type: application/json" \
  -d '{
    "numeroPedido": "PED-123",
    "valor": 100.50,
    "metodoPagamento": "Cartão de Crédito",
    "numeroCartao": "1234-5678-9012-3456"
  }'
```

---

## 12. Boas Práticas

### ✅ DO (Faça)

1. **Use Result<T> para retornos**
   ```csharp
   return Result<MyResponse>.Success(data);
   return Result<MyResponse>.Error("Mensagem de erro");
   ```

2. **Valide no Handler**
   ```csharp
   if (request.Valor <= 0)
       return Result<T>.Error("Valor inválido");
   ```

3. **Use Logging estruturado**
   ```csharp
   _logger.LogInformation("Processando {Entity} com Id {Id}", 
       nameof(Pagamento), pagamento.Id);
   ```

4. **Documente Controllers**
   ```csharp
   /// <summary>
   /// Processa um pagamento
   /// </summary>
   /// <param name="request">Dados do pagamento</param>
   /// <returns>Resultado do processamento</returns>
   [HttpPost]
   public async Task<IActionResult> ProcessarPagamento(...)
   ```

5. **Use CancellationToken**
   ```csharp
   public async Task<Result<T>> Handle(
       Request request, 
       CancellationToken cancellationToken)
   ```

### ❌ DON'T (Não Faça)

1. **Não coloque lógica de negócio no Controller**
   ```csharp
   // ❌ ERRADO
   [HttpPost]
   public IActionResult Post(Request req)
   {
       var entity = new Entity { /* ... */ };
       _db.Save(entity); // Lógica no controller
   }
   
   // ✅ CORRETO
   [HttpPost]
   public async Task<IActionResult> Post(Request req)
   {
       var result = await _mediator.Send(req);
       return result.IsSuccess ? Ok(result) : BadRequest(result);
   }
   ```

2. **Não retorne exceções ao cliente**
   ```csharp
   // ❌ ERRADO
   catch (Exception ex)
   {
       return BadRequest(ex.Message);
   }
   
   // ✅ CORRETO
   catch (Exception ex)
   {
       _logger.LogError(ex, "Erro ao processar");
       return Result<T>.Error("Erro ao processar requisição");
   }
   ```

3. **Não acople a infraestrutura ao domínio**
   ```csharp
   // ❌ ERRADO - Entity com atributos de banco
   public class Pagamento : Entity
   {
       [Column("valor_pagamento")]
       public decimal Valor { get; set; }
   }
   
   // ✅ CORRETO - Use Models na camada de infraestrutura
   ```

---

## Estrutura de Arquivos Completa para uma Feature

```
UseCases/
  ProcessarPagamento/
    Request/
      ProcessarPagamentoRequest.cs
    Response/
      ProcessarPagamentoResponse.cs
    Handler.cs
    
Handlers/
  PagamentoProcessado/
    NotificarClienteHandler.cs
    EnviarEmailHandler.cs
    
Domain/
  Entities/
    Pagamento.cs
  Enums/
    StatusPagamento.cs
  Messages/
    PagamentoProcessadoMessage.cs
    
Controllers/
  PagamentosController.cs
  
Infraestrutura/
  Services/
    PagamentoService.cs
  Models/
    PagamentoModel.cs
    
Mappings/
  AutoMapperProfile.cs (adicionar mapeamentos)
```

---

**Agora você está pronto para construir features completas seguindo a arquitetura! 🚀**
