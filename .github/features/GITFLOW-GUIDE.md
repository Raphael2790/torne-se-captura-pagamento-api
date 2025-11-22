# GitFlow - Guia de Workflows

## Estrutura de Branches e Workflows

Este projeto utiliza **GitFlow** com workflows automatizados do GitHub Actions para garantir qualidade de código e deploy seguro.

## Workflows Configurados

### 1. PR Validation and Creation (`pr-validation.yml`)

**Trigger:** Push para branches `feature/**`, `bug/**`, `fix/**`

**Objetivo:** Validar código e criar Pull Request automaticamente

#### Jobs

##### Validate
- ✅ Checkout do código
- ✅ Setup .NET 8.0
- ✅ Restore de dependências
- ✅ Build do projeto
- ✅ Execução de testes unitários
- ✅ Geração de relatório de cobertura
- ✅ Upload de artefatos de cobertura

##### Create Pull Request
- 🔍 Extrai informações da branch (tipo e título)
- 🔍 Verifica se PR já existe
- ✨ Cria PR com template completo se não existir
- 💬 Adiciona comentário com status da validação
- 🏷️ Adiciona labels automáticas baseadas no tipo

##### Summary
- 📊 Gera resumo do workflow
- ✅ Status da validação
- ✅ Status da criação do PR

### 2. Deploy to AWS Lambda (`deploy.yml`)

**Trigger:** Push para branch `main` ou manual

**Objetivo:** Deploy completo na AWS Lambda (Produção)

#### Jobs

##### Build and Test
- ✅ Build e testes completos
- ✅ Validação antes do deploy

##### Package and Deploy
- 📦 Empacotamento da aplicação
- ☁️ Upload para S3
- 🚀 Deploy na Lambda
- ⚙️ Configuração de variáveis de ambiente
- 📌 Publicação de versão
- 📊 Summary do deployment

## Fluxo de Trabalho GitFlow

### 1. Criar Nova Feature

```bash
# Criar branch de feature
git checkout -b feature/nome-da-feature

# Fazer suas alterações
git add .
git commit -m "feat: implementar nova funcionalidade"

# Push para o repositório
git push origin feature/nome-da-feature
```

**O que acontece:**
- ✅ Workflow `pr-validation.yml` é executado
- ✅ Testes são executados
- ✅ PR é criado automaticamente para `main`
- 📝 Template de PR é preenchido
- 🏷️ Labels são adicionadas (`enhancement`, `feature`)

### 2. Corrigir Bug

```bash
# Criar branch de bug
git checkout -b bug/nome-do-bug

# Fazer correções
git add .
git commit -m "fix: corrigir problema X"

# Push
git push origin bug/nome-do-bug
```

**O que acontece:**
- ✅ Workflow `pr-validation.yml` é executado
- ✅ Testes são executados
- ✅ PR é criado automaticamente
- 🏷️ Labels: `bug`, `bugfix`

### 3. Hotfix/Fix

```bash
# Criar branch de fix
git checkout -b fix/nome-do-fix

# Fazer correções
git add .
git commit -m "fix: correção urgente"

# Push
git push origin fix/nome-do-fix
```

**O que acontece:**
- ✅ Workflow `pr-validation.yml` é executado
- ✅ Testes são executados
- ✅ PR é criado automaticamente
- 🏷️ Labels: `fix`, `hotfix`

### 4. Merge para Main (Deploy)

Após aprovação do PR:

```bash
# Via GitHub UI: Merge Pull Request
# ou via CLI:
gh pr merge NÚMERO_DO_PR --merge
```

**O que acontece:**
- ✅ Workflow `deploy.yml` é executado
- ✅ Build e testes completos
- 📦 Empacotamento da aplicação
- ☁️ Upload para S3
- 🚀 Deploy na AWS Lambda
- 📌 Nova versão publicada

## Nomenclatura de Branches

### Feature Branches
```
feature/adicionar-validacao-webhook
feature/integrar-novo-gateway
feature/melhorar-logging
```

### Bug Branches
```
bug/corrigir-erro-sqs
bug/resolver-timeout-lambda
bug/fix-memory-leak
```

### Fix Branches (Hotfix)
```
fix/corrigir-producao-urgente
fix/ajustar-configuracao
fix/patch-seguranca
```

## Template de Pull Request

Quando um PR é criado automaticamente, ele contém:

```markdown
## [Emoji] [Tipo]: [Título]

### Descrição
<!-- Descreva as mudanças realizadas -->

### Tipo de Mudança
- [ ] 🐛 Bug fix
- [ ] ✨ Nova feature
- [ ] 🔧 Fix
- [ ] 📝 Documentação
- [ ] ♻️ Refatoração
- [ ] ⚡ Performance
- [ ] ✅ Testes

### Checklist
- [ ] Código compilando
- [ ] Testes passando
- [ ] Código revisado
- [ ] Documentação atualizada
- [ ] Sem conflitos com main

### Testes Realizados
<!-- Descreva os testes -->

### Screenshots/Logs
<!-- Se aplicável -->
```

## Labels Automáticas

| Tipo de Branch | Labels Aplicadas | Emoji |
|----------------|------------------|-------|
| `feature/**` | `enhancement`, `feature` | ✨ |
| `bug/**` | `bug`, `bugfix` | 🐛 |
| `fix/**` | `fix`, `hotfix` | 🔧 |

## Comentários Automáticos

### Novo PR Criado
```
✅ Validação Automática Concluída

- ✅ Build: Sucesso
- ✅ Testes Unitários: Todos passando
- ✅ Cobertura de Código: Disponível nos artefatos

Este PR está pronto para revisão! 🚀
```

### Push em PR Existente
```
🔄 Novo Push Detectado

Validação automática executada com sucesso:
- ✅ Build: OK
- ✅ Testes: Passando
- 📊 Commit: [SHA]
```

## Cobertura de Código

O workflow `pr-validation.yml` gera relatórios de cobertura:

- 📊 Formato: XPlat Code Coverage
- 📁 Localização: Artefatos do workflow
- ⏱️ Retenção: 7 dias
- 📥 Download: Actions → Workflow Run → Artifacts

## Boas Práticas

### ✅ Fazer

1. **Criar branches com prefixos corretos** (`feature/`, `bug/`, `fix/`)
2. **Commits semânticos** (`feat:`, `fix:`, `docs:`, etc.)
3. **Aguardar validação** antes de solicitar review
4. **Revisar checklist do PR** antes de marcar como pronto
5. **Resolver conflitos** antes de merge
6. **Testar localmente** antes de push

### ❌ Evitar

1. ~~Fazer commit direto na `main`~~
2. ~~Push sem executar testes localmente~~
3. ~~Ignorar falhas de validação~~
4. ~~Merge sem aprovação~~
5. ~~Branches sem prefixo correto~~
6. ~~Deixar PRs abertos indefinidamente~~

## Comandos Úteis

### Verificar Status Local
```bash
# Ver branch atual
git branch --show-current

# Ver status
git status

# Ver commits não pushados
git log origin/main..HEAD
```

### Sincronizar com Main
```bash
# Atualizar main local
git checkout main
git pull origin main

# Voltar para sua branch
git checkout feature/sua-feature

# Rebase com main (recomendado)
git rebase main

# Ou merge (alternativa)
git merge main
```

### Resolver Conflitos
```bash
# Após rebase/merge com conflitos
git status  # Ver arquivos em conflito

# Editar arquivos e resolver conflitos

git add .
git rebase --continue  # Se estava em rebase
# ou
git commit  # Se estava em merge
```

### Gerenciar PRs via CLI
```bash
# Listar PRs
gh pr list

# Ver detalhes de um PR
gh pr view NÚMERO

# Fazer checkout de um PR
gh pr checkout NÚMERO

# Aprovar PR
gh pr review NÚMERO --approve

# Fazer merge
gh pr merge NÚMERO --merge
```

## Troubleshooting

### Workflow não executou

**Problema:** Push feito mas workflow não foi trigado

**Soluções:**
1. Verificar nome da branch (deve ter prefixo `feature/`, `bug/` ou `fix/`)
2. Verificar se workflows estão habilitados no repositório
3. Verificar permissões do repositório

### PR não foi criado automaticamente

**Problema:** Workflow executou mas PR não aparece

**Soluções:**
1. Verificar se já existe PR para essa branch
2. Verificar permissões do token GITHUB_TOKEN
3. Verificar logs do job `create-pull-request`
4. Criar PR manualmente: `gh pr create --base main`

### Testes falhando no workflow mas passam localmente

**Problema:** Diferenças de ambiente

**Soluções:**
1. Verificar versão do .NET (deve ser 8.0.x)
2. Verificar dependências de testes
3. Executar `dotnet clean` e `dotnet restore`
4. Verificar variáveis de ambiente

### Conflitos com Main

**Problema:** Branch desatualizada

**Soluções:**
```bash
git checkout main
git pull origin main
git checkout sua-branch
git rebase main
# Resolver conflitos
git push --force-with-lease
```

## Métricas e Monitoramento

### Acompanhar Workflows

1. Acesse `Actions` no GitHub
2. Selecione o workflow desejado
3. Visualize logs, tempos de execução e artefatos

### Métricas Importantes

- ⏱️ **Tempo médio de validação**: ~3-5 minutos
- ⏱️ **Tempo médio de deploy**: ~8-12 minutos
- ✅ **Taxa de sucesso**: Objetivo >95%
- 📊 **Cobertura de código**: Objetivo >80%

## Referências

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [GitFlow Workflow](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow)
- [Semantic Versioning](https://semver.org/)
- [Conventional Commits](https://www.conventionalcommits.org/)
