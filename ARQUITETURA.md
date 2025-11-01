# Arquitetura do Projeto - TorneSe.CapturaPagamento.Api

## Diagrama de Camadas

```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                        │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │ Controllers  │  │ Middlewares  │  │  Program.cs  │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                   APPLICATION LAYER                          │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │  UseCases    │  │   Handlers   │  │   Mappings   │      │
│  │   (CQRS)     │  │   (Events)   │  │ (AutoMapper) │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
│                                                               │
│  ┌──────────────┐  ┌──────────────┐                         │
│  │  Extensions  │  │    Common    │                         │
│  │ (DI Config)  │  │   (Result)   │                         │
│  └──────────────┘  └──────────────┘                         │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                     DOMAIN LAYER                             │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │  Entities    │  │   Messages   │  │    Enums     │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
│                                                               │
│  ┌──────────────┐  ┌──────────────┐                         │
│  │  Constants   │  │ Abstracoes   │                         │
│  └──────────────┘  │ (Interfaces) │                         │
│                    └──────────────┘                          │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                 INFRASTRUCTURE LAYER                         │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │   Services   │  │    Models    │  │Configuration │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
```

## Fluxo de Requisição

```
1. HTTP Request
        ↓
2. Middleware (ExceptionHandler)
        ↓
3. Controller
        ↓
4. Use Case (via MediatR)
        ↓
5. Handler
        ↓
6. Domain Logic + Entities
        ↓
7. Repository/Service (via Interface)
        ↓
8. Infrastructure (Implementation)
        ↓
9. External Service / Database
        ↓
10. Response (Result<T>)
```

## Responsabilidades por Camada

### 🎨 Presentation Layer
- **Controllers**: Recebem requisições HTTP, validam entrada
- **Middlewares**: Tratamento de exceções, logging, autenticação
- **Program.cs**: Configuração da aplicação e pipeline

### 💼 Application Layer
- **UseCases**: Implementam casos de uso (Commands/Queries)
- **Handlers**: Processam comandos e eventos
- **Mappings**: Conversão entre DTOs e Entidades
- **Extensions**: Configurações modulares de serviços
- **Common**: Utilitários compartilhados (Result pattern)

### 🏛️ Domain Layer
- **Entities**: Entidades de negócio com comportamento
- **Messages**: Eventos de domínio
- **Enums**: Valores enumerados do domínio
- **Constants**: Constantes da aplicação
- **Abstracoes**: Interfaces para inversão de dependência

### 🔧 Infrastructure Layer
- **Services**: Implementações de serviços externos
- **Models**: Models específicos de persistência
- **Configuration**: Configurações de infraestrutura

## Padrões de Design Utilizados

### 🎯 Arquiteturais
- **Clean Architecture**: Separação de responsabilidades
- **CQRS**: Command Query Responsibility Segregation (via MediatR)
- **Domain-Driven Design**: Domínio no centro

### 🔨 Criacionais
- **Dependency Injection**: Container nativo do .NET
- **Factory Pattern**: Criação de objetos (implícito em services)

### 🔄 Comportamentais
- **Mediator Pattern**: MediatR para desacoplamento
- **Repository Pattern**: Abstração de persistência
- **Strategy Pattern**: Diferentes implementações via interfaces

### 🏗️ Estruturais
- **Adapter Pattern**: Integração com serviços externos
- **Decorator Pattern**: Middlewares em pipeline
- **Facade Pattern**: Extensions simplificam configurações

## Princípios SOLID

### S - Single Responsibility Principle
✅ Cada classe tem uma única responsabilidade
- Controllers apenas recebem requisições
- Use Cases contêm lógica de aplicação
- Entities contêm lógica de domínio

### O - Open/Closed Principle
✅ Aberto para extensão, fechado para modificação
- Uso de interfaces permite novas implementações
- Extension methods adicionam funcionalidades sem modificar classes

### L - Liskov Substitution Principle
✅ Subtipos substituíveis por tipos base
- Entity base pode ser substituída por qualquer entidade
- Message base pode ser substituída por qualquer mensagem

### I - Interface Segregation Principle
✅ Interfaces específicas e coesas
- IDbService: apenas operações de banco
- IMessageService: apenas operações de mensageria

### D - Dependency Inversion Principle
✅ Dependência de abstrações, não implementações
- Controllers dependem de IMediator
- Handlers dependem de interfaces de serviços
- Infraestrutura implementa interfaces do domínio

## Benefícios da Arquitetura

### 🚀 Manutenibilidade
- Código organizado e fácil de encontrar
- Separação clara de responsabilidades
- Baixo acoplamento entre camadas

### 🧪 Testabilidade
- Domain sem dependências externas
- Interfaces facilitam mocking
- Casos de uso isolados e testáveis

### 📈 Escalabilidade
- Novos use cases facilmente adicionados
- Serviços desacoplados
- Infraestrutura intercambiável

### 🔒 Segurança
- Middleware centralizado para tratamento de erros
- Validações em múltiplas camadas
- Logs estruturados

### 👥 Colaboração
- Estrutura padrão facilita onboarding
- Código auto-documentado
- Seguindo convenções da comunidade .NET

---

**Arquitetura profissional, escalável e manutenível! 🎉**
