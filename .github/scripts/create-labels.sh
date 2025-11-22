#!/bin/bash
# Script para criar labels do GitFlow no GitHub
# Execute: ./create-labels.sh ou bash create-labels.sh

echo "🏷️  Criando labels do GitFlow no repositório..."
echo ""

# Features
echo "📦 Features..."
gh label create "enhancement" --color "0E8A16" --description "✨ Melhorias e novas funcionalidades gerais" --force 2>/dev/null && echo "  ✓ enhancement (verde)" || echo "  ✓ enhancement (já existe)"
gh label create "feature" --color "1D76DB" --description "✨ Nova feature específica" --force 2>/dev/null && echo "  ✓ feature (azul)" || echo "  ✓ feature (já existe)"

echo ""
echo "🐛 Bugs..."
gh label create "bug" --color "D73A4A" --description "🐛 Bug reportado" --force 2>/dev/null && echo "  ✓ bug (vermelho)" || echo "  ✓ bug (já existe)"
gh label create "bugfix" --color "E99695" --description "🐛 Correção de bug" --force 2>/dev/null && echo "  ✓ bugfix (rosa)" || echo "  ✓ bugfix (já existe)"

echo ""
echo "🔧 Fixes..."
gh label create "fix" --color "FBCA04" --description "🔧 Correção geral" --force 2>/dev/null && echo "  ✓ fix (amarelo)" || echo "  ✓ fix (já existe)"
gh label create "hotfix" --color "B60205" --description "🔥 Correção urgente em produção" --force 2>/dev/null && echo "  ✓ hotfix (vermelho escuro)" || echo "  ✓ hotfix (já existe)"

echo ""
echo "✅ Labels criadas com sucesso!"
echo ""
echo "📋 Listar labels:"
gh label list
