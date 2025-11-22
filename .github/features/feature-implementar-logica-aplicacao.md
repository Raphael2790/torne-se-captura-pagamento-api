# Feature: Implementar Captura de Pagamento

## Objetivo

Receber um webhook da Stripe via AWS API Gateway, traduzir a mensagem para um objeto de domínio e enviar o evento para uma fila AWS SQS.

### Contexto

- A aplicação deve ser executada como uma AWS Lambda integrada ao API Gateway (HTTP API).
- Não deve haver persistência de dados (sem DynamoDB ou ORM).
- A estrutura, organização e boas práticas devem seguir o repositório de referência `Raphael2790/app-torne-se-pedidos-api`, **sem copiar nenhuma lógica de negócio**.
- Use o comando #githubRepo para analisar o repositório

---

### Passos

1. **Organizar Estrutura da Aplicação**
   - Reestruturar a aplicação para seguir o mesmo padrão do repositório de referência.
   - Adaptar apenas itens de organização, arquitetura e boas práticas, sem reutilizar lógica de negócio existente.

2. **Remover Persistência**
   - Excluir quaisquer camadas ou componentes relacionados a repositórios, contextos ou integração com banco de dados.

3. **Configurar Variáveis de Ambiente**
   - Definir as seguintes variáveis:
     - `AWS_REGION`
     - `SQS_QUEUE_URL`
     - `APP_ENV`
     - `LOG_LEVEL`
     - `STRIPE_SIGNING_SECRET` (opcional em ambiente de desenvolvimento)

4. **Configurar Lambda Hosting**
   - Habilitar execução como Lambda em ambiente AWS, utilizando o modelo de hospedagem HTTP API Gateway.

5. **Criar Endpoint de Webhook**
   - Implementar o endpoint `POST /webhooks/stripe` responsável por receber os payloads enviados pela Stripe.

6. **Capturar Cabeçalho de Assinatura**
   - Ler o cabeçalho `Stripe-Signature` e aplicar validação conforme o ambiente (`APP_ENV`).

7. **Definir DTO de Entrada**
   - Criar o objeto `StripeEventDto` representando o payload recebido, contendo `id`, `type` e o conteúdo principal em `data.object`.

8. **Mapear DTO para Objeto de Domínio**
   - Converter o `StripeEventDto` em um objeto de domínio `PaymentEvent`, contendo apenas as informações necessárias para o processamento.

9. **Criar Comando de Publicação**
   - Definir um comando responsável por encapsular os dados do evento de pagamento antes de enviá-los para a fila SQS.

10. **Criar Serviço de Publicação**
    - Definir uma interface para o serviço responsável pela publicação de eventos de pagamento.

11. **Implementar Publisher SQS**
    - Implementar o componente responsável por enviar o evento serializado para a fila SQS definida em `SQS_QUEUE_URL`.

12. **Adicionar Validações de Entrada**
    - Implementar validações básicas do payload recebido e retornar erros adequados em caso de formato inválido.

13. **Adicionar Observabilidade**
    - Configurar logs padronizados e respostas de erro seguindo o padrão do repositório de referência.

14. **Adicionar Endpoint de Saúde**
    - Criar rota `/health` para verificação de status e disponibilidade da aplicação.

15. **Criar Arquivos de Requisição (.http)**
    - Adicionar arquivos para testes manuais do endpoint:
      - `stripe-payment_succeeded.http`
      - `stripe-charge_refunded.http`
      - `stripe-generic.http`

16. **Criar Testes Automatizados**
    - Adicionar testes unitários para o mapeamento de `DTO → Domínio` e para a publicação na SQS (mock).

17. **Gerar Scripts de Empacotamento**
    - Criar pasta `deploy/` contendo:
      - `package-linux.sh`
      - `package-windows.ps1`
      - `README.md` com instruções e variáveis necessárias.

18. **Configurar Pipeline de CI/CD**
    - Adicionar workflow em `.github/workflows/` com etapas para:
      - Restaurar dependências
      - Executar build e testes
      - Empacotar a aplicação
      - Publicar artefato no S3
      - Criar/atualizar a Lambda .NET 8 com o pacote do S3

19. **Documentar Variáveis e Secrets**
    - Atualizar o arquivo `deploy/README.md` com a lista de variáveis e secrets necessários para execução do workflow.

---

### Critérios de Aceite:

- O endpoint `/webhooks/stripe` retorna **202 Accepted** após enviar o evento para a fila SQS.  
- O evento é publicado corretamente na SQS.  
- Os builds e testes executam com sucesso localmente e no CI/CD.  
- A Lambda é atualizada automaticamente com o pacote publicado no S3.

---

### Restrições:

- Nenhum código de persistência ou repositório deve ser adicionado.  
- Nenhuma lógica de negócio deve ser herdada do repositório de referência.  
- Seguir apenas as convenções, arquitetura e práticas de estrutura previamente definidas.
