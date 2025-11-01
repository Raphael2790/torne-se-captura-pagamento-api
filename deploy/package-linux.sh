#!/bin/bash

# Script para empacotar a aplicação para AWS Lambda (Linux x64)

set -e

echo "===================================="
echo "Package Script - AWS Lambda (Linux)"
echo "===================================="

# Configurações
PROJECT_PATH="./src/TorneSe.CapturaPagamento.Api"
OUTPUT_PATH="./publish/linux-x64"
ZIP_NAME="captura-pagamento-lambda.zip"

echo ""
echo "Limpando diretórios anteriores..."
rm -rf "$OUTPUT_PATH"
rm -f "./publish/$ZIP_NAME"

echo ""
echo "Publicando aplicação para linux-x64..."
dotnet publish "$PROJECT_PATH" \
    --configuration Release \
    --runtime linux-x64 \
    --self-contained false \
    --output "$OUTPUT_PATH" \
    /p:GenerateRuntimeConfigurationFiles=true \
    /p:PublishReadyToRun=true

echo ""
echo "Criando pacote ZIP..."
cd "$OUTPUT_PATH"
zip -r "../../$ZIP_NAME" .
cd ../../

echo ""
echo "===================================="
echo "Pacote criado com sucesso!"
echo "Arquivo: ./publish/$ZIP_NAME"
echo "===================================="

echo ""
echo "Para fazer deploy:"
echo "1. Acesse o console AWS Lambda"
echo "2. Faça upload do arquivo $ZIP_NAME"
echo "3. Configure as variáveis de ambiente necessárias (veja deploy/README.md)"
echo ""
