# Boas Práticas de Programação

## Princípios Fundamentais

### SOLID

Aplique os cinco princípios SOLID em todo o código:

- **S - Single Responsibility Principle (Princípio da Responsabilidade Única)**
  - Cada classe/módulo deve ter apenas uma razão para mudar
  - Uma classe deve ter apenas uma responsabilidade bem definida
  - Evite classes que fazem muitas coisas diferentes

- **O - Open/Closed Principle (Princípio Aberto/Fechado)**
  - Entidades devem estar abertas para extensão, mas fechadas para modificação
  - Use abstrações, interfaces e herança para permitir extensibilidade
  - Evite modificar código existente que já funciona

- **L - Liskov Substitution Principle (Princípio da Substituição de Liskov)**
  - Objetos de uma classe derivada devem poder substituir objetos da classe base
  - Subtipos devem ser substituíveis por seus tipos base sem quebrar a aplicação
  - Mantenha contratos e comportamentos esperados nas heranças

- **I - Interface Segregation Principle (Princípio da Segregação de Interface)**
  - Clientes não devem ser forçados a depender de interfaces que não utilizam
  - Crie interfaces específicas e coesas ao invés de interfaces genéricas
  - Prefira muitas interfaces específicas a uma interface geral

- **D - Dependency Inversion Principle (Princípio da Inversão de Dependência)**
  - Módulos de alto nível não devem depender de módulos de baixo nível
  - Ambos devem depender de abstrações
  - Use injeção de dependência sempre que possível

### YAGNI (You Aren't Gonna Need It)

- Não implemente funcionalidades até que sejam realmente necessárias
- Evite código especulativo ou "por precaução"
- Foque no que é necessário agora, não no que pode ser necessário no futuro
- Mantenha o código simples e não adicione complexidade desnecessária

### KISS (Keep It Simple, Stupid)

- Prefira soluções simples e diretas
- Evite over-engineering e complexidade desnecessária
- Código simples é mais fácil de entender, manter e debugar
- Se existir uma solução simples, use-a ao invés de uma complexa

### DRY (Don't Repeat Yourself)

- Evite duplicação de código
- Extraia código repetido para funções, métodos ou classes reutilizáveis
- Use herança, composição ou funções auxiliares para eliminar repetição
- Cada conhecimento deve ter uma representação única no sistema

## Clean Code

### Nomenclatura

- Use nomes descritivos e significativos
- Nomes de variáveis devem revelar intenção
- Nomes de funções devem ser verbos ou frases verbais
- Nomes de classes devem ser substantivos
- Evite abreviações obscuras
- Use nomenclatura consistente em todo o projeto

### Funções

- Funções devem ser pequenas (idealmente menos de 20 linhas)
- Funções devem fazer apenas uma coisa
- Use parâmetros com moderação (idealmente menos de 3)
- Evite efeitos colaterais
- Prefira retornar novos valores ao invés de modificar parâmetros
- Extraia blocos try/catch para funções separadas

### Comentários

- Código bom deve ser auto-explicativo
- Use comentários apenas quando necessário para explicar "porquê", não "o quê"
- Evite comentários redundantes ou óbvios
- Mantenha comentários atualizados com o código
- Use documentação estruturada para APIs públicas

### Formatação

- Mantenha formatação consistente
- Use indentação adequada
- Agrupe código relacionado
- Separe conceitos com linhas em branco
- Limite o tamanho das linhas (geralmente 80-120 caracteres)
- Siga o guia de estilo da linguagem utilizada

### Tratamento de Erros

- Use exceções ao invés de códigos de retorno
- Forneça contexto nas exceções
- Defina classes de exceção personalizadas quando apropriado
- Não retorne null, prefira Optional/Maybe ou lançar exceções
- Use try-with-resources ou equivalentes para gerenciar recursos

## Documentação

### Documentação de Código

- Documente todas as funções/métodos públicos
- Inclua descrição, parâmetros, retorno e exceções
- Use formato padrão da linguagem (JSDoc, Javadoc, docstrings, etc.)
- Mantenha documentação sincronizada com o código

### README

- Todo projeto deve ter um README.md claro e completo
- Inclua: propósito, instalação, uso, exemplos, contribuição
- Mantenha o README atualizado

### Documentação de APIs

- Documente endpoints, parâmetros, respostas e erros
- Use ferramentas como Swagger/OpenAPI quando aplicável
- Forneça exemplos de uso

## Testes

### Testes Unitários

- Escreva testes para toda lógica de negócio
- Testes devem ser independentes e isolados
- Use padrão AAA: Arrange, Act, Assert
- Um teste deve verificar apenas um comportamento
- Testes devem ser rápidos e determinísticos
- Use mocks/stubs para isolar dependências
- Nomes de testes devem descrever o cenário e resultado esperado
- Use a nomeclatura NomeDoMetodo_Quando_EntaoResultado
- Estrutura de pastas deve refletir a estrutura do código fonte
- Quando necessário reutilize o construtor para inciar os mocks
- Utilize as bibliotecas xUnit e Moq para testes em C#

### Cobertura de Testes

- Busque pelo menos 90% de cobertura de código
- Priorize cobertura de lógica de negócio crítica
- 100% de cobertura não garante ausência de bugs, mas é um bom indicador
- Use ferramentas de cobertura para identificar código não testado
- Não sacrifique qualidade dos testes apenas para aumentar cobertura

## Boas Práticas Gerais

### Controle de Versão

- Commits pequenos e frequentes
- Mensagens de commit claras e descritivas
- Use branches para features e bugfixes
- Revise código antes de fazer merge
- Use convenções como Conventional Commits

### Segurança

- Nunca exponha credenciais no código
- Use variáveis de ambiente para configurações sensíveis
- Valide e sanitize todas as entradas
- Mantenha dependências atualizadas
- Siga princípios de menor privilégio

### Performance

- Não otimize prematuramente
- Use profiling para identificar gargalos reais
- Considere complexidade algorítmica para operações críticas
- Cache quando apropriado, mas com cuidado

### Refatoração

- Refatore regularmente para manter o código limpo
- Refatore em pequenos passos
- Mantenha testes passando durante refatoração
- Remova código morto (dead code)

### Code Review

- Código deve ser revisado antes de ser mergeado
- Reviews devem focar em: legibilidade, correção, design, testes
- Seja construtivo e respeitoso nas revisões

## Checklist antes de Commitar

- [ ] Código segue os princípios SOLID
- [ ] Não há duplicação desnecessária (DRY)
- [ ] Código é simples e direto (KISS)
- [ ] Não há código especulativo (YAGNI)
- [ ] Nomenclatura é clara e consistente
- [ ] Funções são pequenas e focadas
- [ ] Há documentação adequada
- [ ] Testes unitários estão escritos e passando
- [ ] Cobertura de testes é adequada
- [ ] Não há código comentado ou debug leftover
- [ ] Não há credenciais ou dados sensíveis
- [ ] Código está formatado consistentemente

---

**Lembre-se:** Código é lido muito mais vezes do que é escrito. Escreva código pensando em quem vai lê-lo depois, incluindo você mesmo no futuro!