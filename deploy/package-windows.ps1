# Script para empacotar a aplicação para AWS Lambda (Windows)

$ErrorActionPreference = "Stop"

Write-Host "====================================" -ForegroundColor Cyan
Write-Host "Package Script - AWS Lambda (Linux)" -ForegroundColor Cyan
Write-Host "====================================" -ForegroundColor Cyan

# Configurações
$ProjectPath = ".\src\TorneSe.CapturaPagamento.Api"
$OutputPath = ".\publish\linux-x64"
$ZipName = "captura-pagamento-lambda.zip"
$ZipPath = ".\publish\$ZipName"

Write-Host ""
Write-Host "Limpando diretórios anteriores..." -ForegroundColor Yellow
if (Test-Path $OutputPath) {
    Remove-Item -Path $OutputPath -Recurse -Force
}
if (Test-Path $ZipPath) {
    Remove-Item -Path $ZipPath -Force
}

Write-Host ""
Write-Host "Publicando aplicação para linux-x64..." -ForegroundColor Yellow
dotnet publish $ProjectPath `
    --configuration Release `
    --runtime linux-x64 `
    --self-contained false `
    --output $OutputPath `
    /p:GenerateRuntimeConfigurationFiles=true `
    /p:PublishReadyToRun=true

Write-Host ""
Write-Host "Criando pacote ZIP..." -ForegroundColor Yellow
Compress-Archive -Path "$OutputPath\*" -DestinationPath $ZipPath -Force

Write-Host ""
Write-Host "====================================" -ForegroundColor Green
Write-Host "Pacote criado com sucesso!" -ForegroundColor Green
Write-Host "Arquivo: $ZipPath" -ForegroundColor Green
Write-Host "====================================" -ForegroundColor Green

Write-Host ""
Write-Host "Para fazer deploy:" -ForegroundColor Cyan
Write-Host "1. Acesse o console AWS Lambda" -ForegroundColor White
Write-Host "2. Faça upload do arquivo $ZipName" -ForegroundColor White
Write-Host "3. Configure as variáveis de ambiente necessárias (veja deploy\README.md)" -ForegroundColor White
Write-Host ""
