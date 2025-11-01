# TorneSe Captura Pagamento API 💳

API para captura e processamento de pagamentos construída com **ASP.NET Core 8.0** e **AWS Lambda**.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![AWS Lambda](https://img.shields.io/badge/AWS-Lambda-FF9900?logo=amazon-aws)](https://aws.amazon.com/lambda/)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-blue)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
[![SOLID](https://img.shields.io/badge/Principles-SOLID-green)](https://en.wikipedia.org/wiki/SOLID)

## 📋 Sobre o Projeto

Este projeto implementa uma API RESTful para processamento de pagamentos seguindo os princípios de **Clean Architecture** e **SOLID**, com uma estrutura modular e escalável.

## 🏗️ Arquitetura

O projeto está organizado em camadas bem definidas:

- **Presentation Layer**: Controllers, Middlewares
- **Application Layer**: Use Cases (CQRS), Handlers, Extensions
- **Domain Layer**: Entities, Value Objects, Events
- **Infrastructure Layer**: Services, Repositories, External Integrations

Para detalhes completos da arquitetura, consulte [ARQUITETURA.md](./ARQUITETURA.md)

## 🚀 Estrutura do Projeto

```
src/TorneSe.CapturaPagamento.Api/
├── Abstracoes/          # Interfaces e contratos
├── Common/              # Utilitários compartilhados
├── Configuration/       # Configurações da aplicação
├── Controllers/         # Endpoints da API
├── Domain/              # Lógica de negócio
│   ├── Constants/
│   ├── Entities/
│   ├── Enums/
│   └── Messages/
├── Extensions/          # Extension methods
├── Handlers/            # Event handlers
├── Infraestrutura/      # Implementações de infraestrutura
├── Mappings/            # AutoMapper profiles
├── Middlewares/         # Middlewares customizados
└── UseCases/            # Casos de uso (CQRS)
```

## 🛠️ Tecnologias

- **.NET 8.0**
- **ASP.NET Core Web API**
- **AWS Lambda** (Amazon.Lambda.AspNetCoreServer.Hosting)
- **MediatR** - CQRS e eventos de domínio
- **AutoMapper** - Mapeamento de objetos
- **Swagger/OpenAPI** - Documentação interativa da API

## 📦 Pacotes NuGet

```xml
<PackageReference Include="Amazon.Lambda.AspNetCoreServer.Hosting" Version="1.9.0" />
<PackageReference Include="AutoMapper" Version="12.0.1" />
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
<PackageReference Include="MediatR" Version="12.4.1" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.8.1" />
```

## 🎯 Padrões Implementados

### Arquiteturais
- ✅ Clean Architecture
- ✅ CQRS (Command Query Responsibility Segregation)
- ✅ Domain-Driven Design (DDD)

### Design Patterns
- ✅ Repository Pattern
- ✅ Mediator Pattern (via MediatR)
- ✅ Result Pattern
- ✅ Dependency Injection

### Princípios SOLID
- ✅ Single Responsibility Principle
- ✅ Open/Closed Principle
- ✅ Liskov Substitution Principle
- ✅ Interface Segregation Principle
- ✅ Dependency Inversion Principle

## 🚀 Como Executar

### Pré-requisitos
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [AWS Lambda Tools](https://github.com/aws/aws-extensions-for-dotnet-cli) (opcional)

### Executar Localmente

```bash
# Clone o repositório
git clone <repository-url>

# Navegue até o diretório
cd TorneSe.CapturaPagamento.Api/src/TorneSe.CapturaPagamento.Api

# Restaure as dependências
dotnet restore

# Execute a aplicação
dotnet run
```

A API estará disponível em `https://localhost:5001` e o Swagger em `https://localhost:5001/`

### Executar com Docker (futuro)

```bash
docker build -t captura-pagamento-api .
docker run -p 8080:8080 captura-pagamento-api
```

## 📚 Documentação

- **[ARQUITETURA.md](./ARQUITETURA.md)** - Detalhes da arquitetura e camadas
- **[GUIA-DE-USO.md](./GUIA-DE-USO.md)** - Como adicionar novas features
- **[REESTRUTURACAO.md](./REESTRUTURACAO.md)** - Histórico de mudanças da arquitetura

## 🔧 Configuração

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Swagger": {
    "Url": "/swagger/v1/swagger.json"
  }
}
```

## 📖 Swagger/OpenAPI

A documentação interativa da API está disponível via Swagger UI na raiz da aplicação (`/`).

### Endpoints Disponíveis

- `GET /` - Página inicial (redireciona para Swagger)
- `GET /calculator/*` - Endpoints de exemplo (Calculator)

## 🧪 Testes

```bash
# Executar todos os testes
dotnet test

# Executar com cobertura
dotnet test /p:CollectCoverage=true
```

## 📈 Melhorias Futuras

- [ ] Implementar autenticação JWT
- [ ] Adicionar cache distribuído (Redis)
- [ ] Implementar Circuit Breaker
- [ ] Adicionar métricas e observabilidade
- [ ] Implementar rate limiting
- [ ] Adicionar validações com FluentValidation
- [ ] Implementar testes de integração
- [ ] Adicionar HealthChecks

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudanças (`git commit -m 'Adiciona MinhaFeature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abra um Pull Request

## 📝 Convenções de Código

Siga as [Boas Práticas de Programação](./.github/copilot-instructions.md) definidas no projeto.

## 🐛 Reportando Bugs

Encontrou um bug? Abra uma [issue](https://github.com/seu-usuario/TorneSe.CapturaPagamento.Api/issues) descrevendo:
- Descrição do problema
- Passos para reproduzir
- Comportamento esperado
- Screenshots (se aplicável)

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 👥 Autores

- **Seu Nome** - *Desenvolvimento Inicial*

## 🙏 Agradecimentos

- Projeto baseado na arquitetura de [Raphael2790/app-torne-se-pedidos-api](https://github.com/Raphael2790/app-torne-se-pedidos-api)
- Clean Architecture por Robert C. Martin (Uncle Bob)
- Comunidade .NET

---

**Desenvolvido com ❤️ usando .NET 8.0**
