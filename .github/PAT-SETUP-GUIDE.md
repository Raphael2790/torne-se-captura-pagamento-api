# 🔐 Guia de Configuração do Personal Access Token (PAT)

## ❗ Por que preciso de um PAT?

O GitHub Actions tem uma **restrição de segurança** que impede workflows acionados por `push` de criarem Pull Requests usando o `GITHUB_TOKEN` padrão. Isso previne loops infinitos de workflows.

**Erro sem PAT:**
```
pull request create failed: GraphQL: GitHub Actions is not permitted to create or approve pull requests (createPullRequest)
```

## 📋 Passo a Passo

### 1️⃣ Criar Personal Access Token (Fine-grained)

1. **Acesse:** https://github.com/settings/tokens?type=beta

2. **Clique em:** `Generate new token` → `Generate new token (fine-grained)`

3. **Configure o Token:**

   **Token name:** `GitFlow Automation`
   
   **Expiration:** `90 days` (ou escolha sua preferência)
   
   **Description:** `Token para automação de PRs no workflow GitFlow`
   
   **Repository access:**
   - ✅ Selecione: `Only select repositories`
   - ✅ Escolha: `torne-se-captura-pagamento-api`

4. **Permissions (Repository permissions):**

   | Permission | Access | Motivo |
   |------------|--------|--------|
   | **Contents** | ✅ Read-only | Ler código do repositório |
   | **Pull requests** | ✅ Read and write | Criar e gerenciar PRs |
   | **Metadata** | ✅ Read-only | Acesso básico (auto-selecionado) |

5. **Clique em:** `Generate token`

6. **⚠️ IMPORTANTE:** Copie o token gerado AGORA! Você não poderá vê-lo novamente.

   O token terá este formato: `github_pat_11AAAA...`

### 2️⃣ Adicionar Token como Secret no Repositório

1. **Acesse:** https://github.com/Raphael2790/torne-se-captura-pagamento-api/settings/secrets/actions

2. **Clique em:** `New repository secret`

3. **Configure o Secret:**
   - **Name:** `PAT_TOKEN` (exatamente este nome!)
   - **Secret:** Cole o token que você copiou
   
4. **Clique em:** `Add secret`

5. **✅ Confirme:** O secret `PAT_TOKEN` deve aparecer na lista

### 3️⃣ Verificar Configuração

Após adicionar o secret, o workflow está pronto! Os erros de lint no VS Code desaparecerão.

**Teste fazendo um push em qualquer branch feature/bug/fix:**

```powershell
git add .
git commit -m "test: verificar criação automática de PR"
git push
```

O workflow deve:
1. ✅ Validar o código (build + testes)
2. ✅ Criar PR automaticamente (sem erro!)
3. ✅ Adicionar labels (se existirem)
4. ✅ Adicionar comentário de validação

## 🔍 Como Verificar se Está Funcionando

### Via GitHub Actions:
1. Acesse: https://github.com/Raphael2790/torne-se-captura-pagamento-api/actions
2. Procure pelo workflow: `PR Validation and Creation`
3. Clique na execução mais recente
4. Verifique se todos os steps passaram ✅

### Via Pull Requests:
1. Acesse: https://github.com/Raphael2790/torne-se-captura-pagamento-api/pulls
2. Deve haver um novo PR criado automaticamente
3. O PR deve ter:
   - ✅ Título formatado (ex: "✨ Feature: Nome Da Feature")
   - ✅ Descrição com template preenchido
   - ✅ Comentário de validação automática
   - ✅ Labels (se criados anteriormente)

## 🔐 Segurança do PAT

### ✅ Boas Práticas Implementadas:

1. **Fine-grained Token:** Acesso restrito apenas ao repositório específico
2. **Permissions Mínimas:** Apenas o necessário (Contents: read, PRs: read/write)
3. **Expiração:** Token expira automaticamente (renove quando necessário)
4. **Secret:** Token armazenado como secret do GitHub (nunca exposto nos logs)

### 🔄 Renovação do Token

Quando o token expirar (você receberá email), repita o processo:
1. Criar novo token com as mesmas configurações
2. Atualizar o secret `PAT_TOKEN` no repositório

## ❓ Troubleshooting

### Erro: "Resource not accessible by personal access token"

**Causa:** Token sem permissões corretas

**Solução:**
1. Verifique se o token tem: `Pull requests: Read and write`
2. Verifique se o token tem acesso ao repositório correto
3. Recrie o token se necessário

### Erro: "Bad credentials"

**Causa:** Token inválido ou expirado

**Solução:**
1. Verifique a expiração do token em: https://github.com/settings/tokens
2. Gere um novo token
3. Atualize o secret `PAT_TOKEN`

### Workflow não cria PR

**Checklist:**
- [ ] Secret `PAT_TOKEN` existe no repositório?
- [ ] Token tem permissão `Pull requests: Read and write`?
- [ ] Token tem acesso ao repositório?
- [ ] Token não está expirado?
- [ ] Workflow está ativo (não desabilitado)?

## 📚 Documentação Oficial

- [Fine-grained Personal Access Tokens](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens#creating-a-fine-grained-personal-access-token)
- [GitHub Actions Secrets](https://docs.github.com/en/actions/security-guides/encrypted-secrets)
- [GitHub Actions Permissions](https://docs.github.com/en/actions/security-guides/automatic-token-authentication#permissions-for-the-github_token)

## 🎯 Resumo Rápido

```
1. Criar Token → https://github.com/settings/tokens?type=beta
   - Nome: GitFlow Automation
   - Repo: torne-se-captura-pagamento-api
   - Permissions: Contents (read), Pull requests (read/write)

2. Adicionar Secret → repo/settings/secrets/actions
   - Name: PAT_TOKEN
   - Value: [seu token]

3. Testar → git push em branch feature/bug/fix
   - Verificar Actions tab
   - Verificar PR criado automaticamente
```

---

**🎉 Pronto! Após estes passos, seus PRs serão criados automaticamente via GitHub Actions!**
