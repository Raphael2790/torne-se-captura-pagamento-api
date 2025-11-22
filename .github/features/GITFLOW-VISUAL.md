# GitFlow - Resumo Visual

## 📊 Estrutura de Workflows

```
┌─────────────────────────────────────────────────────────────┐
│                    REPOSITÓRIO GITHUB                        │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌─────────────────┐                                        │
│  │  Branch: main   │◄──────────────────┐                   │
│  └────────┬────────┘                    │                   │
│           │                             │                   │
│           │ push                    merge PR                │
│           ▼                             │                   │
│  ┌─────────────────────────────┐       │                   │
│  │   deploy.yml (Production)   │       │                   │
│  ├─────────────────────────────┤       │                   │
│  │ ✅ Build and Test           │       │                   │
│  │ ✅ Package and Deploy       │       │                   │
│  │ ✅ Update Lambda            │       │                   │
│  │ ✅ Publish Version          │       │                   │
│  └─────────────────────────────┘       │                   │
│                                         │                   │
│                                         │                   │
│  ┌────────────────────────────────┐    │                   │
│  │  Feature/Bug/Fix Branches      │────┘                   │
│  ├────────────────────────────────┤                        │
│  │  • feature/**                  │                        │
│  │  • bug/**                      │                        │
│  │  • fix/**                      │                        │
│  └────────┬───────────────────────┘                        │
│           │                                                 │
│           │ push                                            │
│           ▼                                                 │
│  ┌─────────────────────────────────┐                       │
│  │   pr-validation.yml             │                       │
│  ├─────────────────────────────────┤                       │
│  │ ✅ Build and Test               │                       │
│  │ ✅ Code Coverage                │                       │
│  │ ✅ Auto-create PR               │                       │
│  │ ✅ Add Labels                   │                       │
│  │ ✅ Add Comments                 │                       │
│  └─────────────────────────────────┘                       │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

## 🔄 Fluxo de Trabalho

### Cenário 1: Nova Feature

```
Developer                    GitHub Actions                  AWS
    │                              │                          │
    │ 1. git checkout -b           │                          │
    │    feature/new-feature       │                          │
    │                              │                          │
    │ 2. git commit & push         │                          │
    ├─────────────────────────────►│                          │
    │                              │                          │
    │                         ✅ pr-validation.yml            │
    │                              │                          │
    │                         • Build & Test                  │
    │                         • Coverage Report               │
    │                         • Create PR                     │
    │                              │                          │
    │◄─────────────────────────────┤                          │
    │ 3. PR #123 created           │                          │
    │    ✨ feature: New Feature   │                          │
    │                              │                          │
    │ 4. Code Review & Approve     │                          │
    │                              │                          │
    │ 5. Merge PR                  │                          │
    ├─────────────────────────────►│                          │
    │                              │                          │
    │                         ✅ deploy.yml                   │
    │                              │                          │
    │                         • Build & Test                  │
    │                         • Package                       │
    │                         • Upload S3                     │
    │                              ├─────────────────────────►│
    │                              │  Update Lambda           │
    │                              │  Version: v42            │
    │                              │◄─────────────────────────┤
    │                              │                          │
    │◄─────────────────────────────┤                          │
    │ 6. Deploy successful ✅       │                          │
    │    Version: v42              │                          │
```

### Cenário 2: Bug Fix

```
Developer                    GitHub Actions                  AWS
    │                              │                          │
    │ 1. git checkout -b           │                          │
    │    bug/fix-issue             │                          │
    │                              │                          │
    │ 2. git commit & push         │                          │
    ├─────────────────────────────►│                          │
    │                              │                          │
    │                         ✅ pr-validation.yml            │
    │                              │                          │
    │◄─────────────────────────────┤                          │
    │ 3. PR #124 created           │                          │
    │    🐛 bug: Fix Issue         │                          │
    │                              │                          │
    │ 4. Fast Review & Merge       │                          │
    ├─────────────────────────────►│                          │
    │                              │                          │
    │                         ✅ deploy.yml                   │
    │                              ├─────────────────────────►│
    │                              │  Hotfix Deploy           │
    │◄─────────────────────────────┤◄─────────────────────────┤
    │ 5. Hotfix deployed ✅         │                          │
```

## 📋 Matriz de Decisão

| Branch Pattern | Workflow Executado | Ação Principal | Deploy na AWS |
|----------------|-------------------|----------------|---------------|
| `main` | `deploy.yml` | Build → Package → Deploy | ✅ Sim |
| `feature/**` | `pr-validation.yml` | Build → Test → Create PR | ❌ Não |
| `bug/**` | `pr-validation.yml` | Build → Test → Create PR | ❌ Não |
| `fix/**` | `pr-validation.yml` | Build → Test → Create PR | ❌ Não |
| Outros | ❌ Nenhum | - | ❌ Não |

## 🏷️ Labels Automáticas

| Branch Type | Labels Aplicadas | Emoji | Cor |
|-------------|------------------|-------|-----|
| `feature/**` | `enhancement`, `feature` | ✨ | 🟢 Verde |
| `bug/**` | `bug`, `bugfix` | 🐛 | 🔴 Vermelho |
| `fix/**` | `fix`, `hotfix` | 🔧 | 🟠 Laranja |

## 💬 Comentários Automáticos

### Quando PR é Criado

```markdown
✅ Validação Automática Concluída

- ✅ Build: Sucesso
- ✅ Testes Unitários: Todos passando
- ✅ Cobertura de Código: Disponível nos artefatos

Este PR está pronto para revisão! 🚀
```

### Quando Novo Push em PR Existente

```markdown
🔄 Novo Push Detectado

Validação automática executada com sucesso:
- ✅ Build: OK
- ✅ Testes: Passando
- 📊 Commit: abc123def456
```

## 📊 Estatísticas dos Workflows

### pr-validation.yml

| Métrica | Valor Típico |
|---------|--------------|
| ⏱️ Tempo de Execução | 3-5 minutos |
| 🔄 Jobs | 3 (validate, create-pr, summary) |
| 📦 Artefatos | Coverage reports |
| ⚡ Acionamento | Push em feature/bug/fix branches |

### deploy.yml

| Métrica | Valor Típico |
|---------|--------------|
| ⏱️ Tempo de Execução | 8-12 minutos |
| 🔄 Jobs | 2 (build-test, package-deploy) |
| 📦 Artefatos | Lambda package (.zip) |
| ⚡ Acionamento | Push em main ou manual |

## 🎯 Gatilhos dos Workflows

```yaml
# pr-validation.yml
on:
  push:
    branches:
      - 'feature/**'
      - 'bug/**'
      - 'fix/**'
  workflow_dispatch:

# deploy.yml
on:
  push:
    branches:
      - main
  workflow_dispatch:
    inputs:
      environment:
        type: choice
        options:
          - production
```

## 🔐 Secrets Necessários

### Para Deploy (deploy.yml)

| Secret | Obrigatório | Uso |
|--------|-------------|-----|
| `AWS_ACCESS_KEY_ID` | ✅ Sim | Credenciais AWS |
| `AWS_SECRET_ACCESS_KEY` | ✅ Sim | Credenciais AWS |
| `SQS_QUEUE_URL` | ✅ Sim | URL da fila SQS |
| `APP_ENV` | ✅ Sim | Ambiente (Production) |
| `LOG_LEVEL` | ✅ Sim | Nível de log |
| `STRIPE_SIGNING_SECRET` | ✅ Sim | Secret do Stripe |

### Para PR Validation (pr-validation.yml)

| Secret | Obrigatório | Uso |
|--------|-------------|-----|
| `GITHUB_TOKEN` | ✅ Sim | Auto-fornecido pelo GitHub |

## 📈 Fluxo de Estados do PR

```
┌─────────────┐
│ Push Branch │
└──────┬──────┘
       │
       ▼
┌─────────────────┐
│ Run Validation  │
└──────┬──────────┘
       │
       ├─────► ❌ Failed → Fix & Push Again
       │
       ▼
   ✅ Success
       │
       ▼
┌─────────────────┐
│  Create/Update  │
│      PR         │
└──────┬──────────┘
       │
       ▼
┌─────────────────┐
│  Code Review    │
└──────┬──────────┘
       │
       ├─────► 💬 Request Changes → Fix & Push
       │
       ├─────► ✅ Approve
       │
       ▼
┌─────────────────┐
│    Merge PR     │
└──────┬──────────┘
       │
       ▼
┌─────────────────┐
│  Deploy (main)  │
└──────┬──────────┘
       │
       ▼
   🚀 Production
```

## 🛠️ Comandos Rápidos

### Criar Feature

```bash
git checkout -b feature/minha-feature
git commit -m "feat: implementar nova funcionalidade"
git push origin feature/minha-feature
# PR é criado automaticamente
```

### Verificar Status do PR

```bash
gh pr list
gh pr view 123
gh pr checks 123
```

### Merge e Deploy

```bash
# Via GitHub UI ou:
gh pr merge 123 --merge
# Deploy automático inicia
```

## 📚 Documentação Relacionada

- 📖 [Guia Completo do GitFlow](GITFLOW-GUIDE.md)
- 🚀 [Workflow GitHub Actions](WORKFLOW-GITHUB-ACTIONS.md)
- 📦 [Guia de Deploy](../../deploy/README.md)

## 🎓 Treinamento Rápido

### Para Desenvolvedores

1. ✅ Sempre criar branch com prefixo correto
2. ✅ Aguardar validação automática passar
3. ✅ PR será criado automaticamente
4. ✅ Preencher checklist do PR
5. ✅ Solicitar review
6. ✅ Fazer merge após aprovação

### Para Revisores

1. ✅ Verificar se validação passou
2. ✅ Revisar código e checklist
3. ✅ Aprovar ou solicitar mudanças
4. ✅ Merge após aprovação

### Para DevOps

1. ✅ Configurar secrets no GitHub
2. ✅ Criar Lambda e SQS na AWS
3. ✅ Monitorar workflows no Actions
4. ✅ Revisar logs de deploy
