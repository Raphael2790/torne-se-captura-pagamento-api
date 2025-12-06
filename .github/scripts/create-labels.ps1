# Script para criar labels do GitFlow no GitHub
# Execute: .\create-labels.ps1

Write-Host "🏷️  Criando labels do GitFlow no repositório..." -ForegroundColor Cyan
Write-Host ""

# Features
Write-Host "📦 Features..." -ForegroundColor Yellow
gh label create "enhancement" --color "0E8A16" --description "✨ Melhorias e novas funcionalidades gerais" --force 2>$null
if ($LASTEXITCODE -eq 0) { Write-Host "  ✓ enhancement (verde)" -ForegroundColor Green } else { Write-Host "  ✓ enhancement (já existe)" -ForegroundColor Gray }

gh label create "feature" --color "1D76DB" --description "✨ Nova feature específica" --force 2>$null
if ($LASTEXITCODE -eq 0) { Write-Host "  ✓ feature (azul)" -ForegroundColor Green } else { Write-Host "  ✓ feature (já existe)" -ForegroundColor Gray }

Write-Host ""
Write-Host "🐛 Bugs..." -ForegroundColor Yellow
gh label create "bug" --color "D73A4A" --description "🐛 Bug reportado" --force 2>$null
if ($LASTEXITCODE -eq 0) { Write-Host "  ✓ bug (vermelho)" -ForegroundColor Green } else { Write-Host "  ✓ bug (já existe)" -ForegroundColor Gray }

gh label create "bugfix" --color "E99695" --description "🐛 Correção de bug" --force 2>$null
if ($LASTEXITCODE -eq 0) { Write-Host "  ✓ bugfix (rosa)" -ForegroundColor Green } else { Write-Host "  ✓ bugfix (já existe)" -ForegroundColor Gray }

Write-Host ""
Write-Host "🔧 Fixes..." -ForegroundColor Yellow
gh label create "fix" --color "FBCA04" --description "🔧 Correção geral" --force 2>$null
if ($LASTEXITCODE -eq 0) { Write-Host "  ✓ fix (amarelo)" -ForegroundColor Green } else { Write-Host "  ✓ fix (já existe)" -ForegroundColor Gray }

gh label create "hotfix" --color "B60205" --description "🔥 Correção urgente em produção" --force 2>$null
if ($LASTEXITCODE -eq 0) { Write-Host "  ✓ hotfix (vermelho escuro)" -ForegroundColor Green } else { Write-Host "  ✓ hotfix (já existe)" -ForegroundColor Gray }

Write-Host ""
Write-Host "✅ Labels criadas com sucesso!" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Listar labels:" -ForegroundColor Cyan
gh label list
