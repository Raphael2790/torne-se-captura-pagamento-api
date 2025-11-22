# Script para Criar Labels do GitHub

Este script cria as labels padrão usadas pelo workflow GitFlow.

## Labels Criadas

### Features
- **enhancement** (Verde) - Melhorias e novas funcionalidades gerais
- **feature** (Verde Escuro) - Nova feature específica

### Bugs
- **bug** (Vermelho) - Bug reportado
- **bugfix** (Laranja Escuro) - Correção de bug

### Fixes
- **fix** (Laranja) - Correção geral
- **hotfix** (Vermelho Escuro) - Correção urgente em produção

## Como Executar

### Via GitHub CLI

```bash
# Features
gh label create "enhancement" --color "0E8A16" --description "Melhorias e novas funcionalidades" || echo "Label já existe"
gh label create "feature" --color "1D76DB" --description "Nova feature" || echo "Label já existe"

# Bugs
gh label create "bug" --color "D73A4A" --description "Bug reportado" || echo "Label já existe"
gh label create "bugfix" --color "E99695" --description "Correção de bug" || echo "Label já existe"

# Fixes
gh label create "fix" --color "FBCA04" --description "Correção geral" || echo "Label já existe"
gh label create "hotfix" --color "B60205" --description "Correção urgente" || echo "Label já existe"
```

### Via Interface Web

1. Acesse: `https://github.com/OWNER/REPO/labels`
2. Clique em "New label"
3. Crie cada label conforme a tabela abaixo:

| Nome | Cor | Descrição |
|------|-----|-----------|
| `enhancement` | #0E8A16 (Verde) | Melhorias e novas funcionalidades |
| `feature` | #1D76DB (Azul) | Nova feature |
| `bug` | #D73A4A (Vermelho) | Bug reportado |
| `bugfix` | #E99695 (Rosa) | Correção de bug |
| `fix` | #FBCA04 (Amarelo) | Correção geral |
| `hotfix` | #B60205 (Vermelho Escuro) | Correção urgente |

## Script Completo (Bash)

```bash
#!/bin/bash

echo "Criando labels do GitFlow..."

# Features
gh label create "enhancement" \
  --color "0E8A16" \
  --description "✨ Melhorias e novas funcionalidades gerais" \
  --force 2>/dev/null || echo "✓ enhancement"

gh label create "feature" \
  --color "1D76DB" \
  --description "✨ Nova feature específica" \
  --force 2>/dev/null || echo "✓ feature"

# Bugs
gh label create "bug" \
  --color "D73A4A" \
  --description "🐛 Bug reportado" \
  --force 2>/dev/null || echo "✓ bug"

gh label create "bugfix" \
  --color "E99695" \
  --description "🐛 Correção de bug" \
  --force 2>/dev/null || echo "✓ bugfix"

# Fixes
gh label create "fix" \
  --color "FBCA04" \
  --description "🔧 Correção geral" \
  --force 2>/dev/null || echo "✓ fix"

gh label create "hotfix" \
  --color "B60205" \
  --description "🔥 Correção urgente em produção" \
  --force 2>/dev/null || echo "✓ hotfix"

echo ""
echo "Labels criadas com sucesso! ✅"
echo ""
echo "Visualize em: https://github.com/$(gh repo view --json nameWithOwner -q .nameWithOwner)/labels"
```

## Script PowerShell

```powershell
Write-Host "Criando labels do GitFlow..." -ForegroundColor Cyan

# Features
gh label create "enhancement" --color "0E8A16" --description "✨ Melhorias e novas funcionalidades gerais" --force 2>$null
if ($LASTEXITCODE -eq 0) { Write-Host "✓ enhancement" -ForegroundColor Green }

gh label create "feature" --color "1D76DB" --description "✨ Nova feature específica" --force 2>$null
if ($LASTEXITCODE -eq 0) { Write-Host "✓ feature" -ForegroundColor Green }

# Bugs
gh label create "bug" --color "D73A4A" --description "🐛 Bug reportado" --force 2>$null
if ($LASTEXITCODE -eq 0) { Write-Host "✓ bug" -ForegroundColor Green }

gh label create "bugfix" --color "E99695" --description "🐛 Correção de bug" --force 2>$null
if ($LASTEXITCODE -eq 0) { Write-Host "✓ bugfix" -ForegroundColor Green }

# Fixes
gh label create "fix" --color "FBCA04" --description "🔧 Correção geral" --force 2>$null
if ($LASTEXITCODE -eq 0) { Write-Host "✓ fix" -ForegroundColor Green }

gh label create "hotfix" --color "B60205" --description "🔥 Correção urgente em produção" --force 2>$null
if ($LASTEXITCODE -eq 0) { Write-Host "✓ hotfix" -ForegroundColor Green }

Write-Host ""
Write-Host "Labels criadas com sucesso! ✅" -ForegroundColor Green
```

## Verificar Labels Criadas

```bash
gh label list
```

## Remover Labels (se necessário)

```bash
# Cuidado! Isso remove as labels
gh label delete "enhancement" --yes
gh label delete "feature" --yes
gh label delete "bug" --yes
gh label delete "bugfix" --yes
gh label delete "fix" --yes
gh label delete "hotfix" --yes
```

## Observação Importante

⚠️ **O workflow pr-validation.yml agora trata labels inexistentes graciosamente**

Se uma label não existir, o workflow:
1. Não falhará
2. Exibirá mensagem informativa
3. Continuará o processo normalmente

Mas é **altamente recomendado** criar as labels para melhor organização dos PRs.

## Troubleshooting

### "Label already exists"
✅ Normal - A label já foi criada anteriormente

### "gh: command not found"
❌ Instale o GitHub CLI: https://cli.github.com/

### "insufficient permissions"
❌ Você precisa ser administrador do repositório ou ter permissões de write

### Labels não aparecem no PR
1. Verifique se as labels foram criadas: `gh label list`
2. Execute o script de criação de labels
3. O workflow adicionará as labels automaticamente no próximo push
