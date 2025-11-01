using TorneSe.CapturaPagamento.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços da aplicação
builder.Services.AddApplicationServices();

// Registrar middleware de tratamento de exceções
builder.Services.AddGlobalExceptionHandler();

// Configurar Swagger/OpenAPI
builder.Services.ConfigureSwagger();

var app = builder.Build();

// Adicionar middleware de tratamento de exceções no início do pipeline
app.UseGlobalExceptionHandler();

// Configurar pipeline da aplicação
app.ConfigureApp();

// Configurar interface do Swagger
app.UseSwaggerInterface();

app.MapGet("/", () => "Welcome to TorneSe Captura Pagamento API - Running on AWS Lambda");

app.Run();
