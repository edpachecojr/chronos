# Instruções canônicas do Chronos

Este arquivo é a fonte principal das regras de engenharia do projeto. Toda
alteração deve respeitá-lo. Quando uma regra deixar de fazer sentido, ela deve
ser discutida e atualizada explicitamente, junto com os ADRs afetados.

## Princípios de implementação

- Cada função deve fazer uma única coisa, fazê-la bem e permanecer em um único
  nível de abstração.
- Funções devem ter, idealmente, entre 4 e 20 linhas. Divida funções maiores por
  responsabilidade, sem criar indireções artificiais.
- Arquivos devem ficar preferencialmente abaixo de 200 linhas. Entre 200 e 300
  linhas é aceitável quando a coesão justificar. Acima disso, divida por
  responsabilidade.
- Aplique o Single Responsibility Principle (SRP): cada módulo deve ter uma
  responsabilidade e uma única razão para mudar.
- Use retornos antecipados e mantenha no máximo dois níveis de indentação.
- Evite indireções desnecessárias, comportamento implícito e lógica "mágica".
- Aplique DRY quando houver conhecimento ou comportamento realmente repetido.
  Extraia lógica compartilhada para funções ou módulos coesos, sem generalizar
  antes de existir uma abstração clara.

## Nomes e tipos

- Escolha nomes específicos, únicos, pronunciáveis, pesquisáveis e que revelem
  intenção.
- Evite nomes genéricos como `data`, `handler` e `Manager`. Prefira nomes cujo
  significado seja inequívoco no domínio e que retornem menos de cinco
  ocorrências em uma busca no código, quando viável.
- Use tipos explícitos em assinaturas e fronteiras. A assinatura deve comunicar
  entradas, saídas e estados válidos.
- Não use `any`, dicionários sem tipo ou funções sem tipagem.
- Entidades têm nomes no singular; tabelas têm nomes no plural e seguem
  `snake_case`.

## Comentários e documentação

- Código claro deve explicar o que faz. Não escreva comentários que apenas
  descrevam o óbvio.
- Comentários devem explicar **por que** uma decisão existe, incluindo contexto
  e proveniência quando necessários.
- Preserve comentários existentes durante refatorações; eles podem carregar
  intenção histórica. Atualize-os quando o comportamento relacionado mudar.
- Funções públicas devem ter documentação de intenção e um exemplo curto de uso.
- Referencie issues ou commits quando uma linha existir por causa de um bug ou
  restrição externa específica.

## Erros e resultados

- Mensagens de erro devem informar o valor ofensivo, o contexto da operação e o
  formato ou estado esperado, sem revelar dados sensíveis.
- Operações com falhas esperadas devem usar Result Pattern, com erros modelados
  como `record`.
- Use exceções de domínio somente para cenários de erro de domínio não tratados
  pelo fluxo esperado.
- Entidades podem produzir eventos de domínio para comunicar fatos relevantes
  sem acoplamento indevido.

## Testes

- Testes devem executar por um único comando, a ser registrado aqui quando a
  estrutura do projeto for criada.
- Toda nova função deve ter teste. Toda correção de bug deve incluir um teste de
  regressão.
- Os testes devem seguir F.I.R.S.T.:
  - **Fast (rápidos):** fornecem retorno imediato;
  - **Independent (independentes):** não dependem da ordem ou de outros testes;
  - **Repeatable (repetíveis):** produzem o mesmo resultado em qualquer ambiente
    controlado;
  - **Self-Validating (autovalidáveis):** indicam sucesso ou falha sem inspeção
    manual;
  - **Timely (oportunos):** são escritos junto com o código que validam.
- Isole I/O externo, como APIs, banco de dados e sistema de arquivos, com classes
  fake nomeadas, não com stubs anônimos em linha.
- Testes unitários devem ser rápidos, determinísticos e validar uma unidade de
  comportamento.

## Dependências e testabilidade

- Injete dependências por construtor ou parâmetro. Não dependa de estado global
  ou de dependências ocultas.
- Encapsule bibliotecas de terceiros atrás de interfaces pequenas pertencentes
  ao projeto quando isso proteger o domínio ou melhorar a testabilidade.
- Dependências externas devem poder ser substituídas por mocks ou fakes nos
  testes.

## Estrutura

- Este repositório é um monorepo. Cada aplicação fica em uma pasta de primeiro
  nível com seu próprio `src/`, `tests/` e arquivo de solução.
- O backend `chronos-agenda` usa `src/Chronos.Agenda.Domain`,
  `src/Chronos.Agenda.Application`, `src/Chronos.Agenda.Infrastructure` e
  `src/Chronos.Agenda.Api`; seus testes ficam em `tests/`.
- O domínio não depende das outras camadas. A aplicação depende do domínio; a
  infraestrutura depende da aplicação; e a API depende da aplicação.
- Siga as convenções dos frameworks escolhidos e mantenha caminhos previsíveis.
- Prefira módulos pequenos e focados; não crie arquivos ou classes que concentrem
  responsabilidades distintas.
- Não introduza uma nova convenção de diretórios sem atualizar este documento.

## Formatação

- Use o formatador padrão de cada linguagem ou ecossistema.
- Não mantenha regras manuais de estilo que conflitem com o formatador adotado.

## Logging e observabilidade

- Logs técnicos devem ser estruturados em JSON e usar campos nomeados.
- Registre contexto suficiente para diagnosticar a operação, sem incluir
  segredos ou dados pessoais desnecessários.
- Texto livre é reservado para saídas diretamente destinadas ao usuário.

## Segurança e configuração

- O repositório é público. Nunca versione segredos, tokens, chaves, senhas,
  credenciais, strings de conexão reais ou dados pessoais.
- Nunca mantenha valores sensíveis hardcoded, nem mesmo em testes, exemplos,
  logs, documentação ou histórico do Git.
- Injete configurações sensíveis por variáveis de ambiente ou por um gerenciador
  de segredos apropriado ao ambiente.
- Use somente valores fictícios e claramente identificados em exemplos.
- Antes de commitar, revise arquivos novos e alterações em busca de material
  sensível.

## Arquitetura vigente

- Consulte os ADRs em `docs/adr` antes de tomar decisões estruturais.
- O ADR inicial define .NET 10, ASP.NET Core Identity, EF Core, PostgreSQL,
  endpoints, Result Pattern, eventos e exceções de domínio e multi-tenancy
  explícita na camada de aplicação.
- Não altere uma decisão aceita silenciosamente. Crie um novo ADR que substitua
  a decisão anterior e atualize a documentação relacionada.

<!-- ai-memory:start -->
## Long-term memory (ai-memory)

This project uses [ai-memory](https://github.com/akitaonrails/ai-memory)
for cross-session continuity.

**Default to the current project - always.** Every ai-memory tool
auto-scopes to the project resolved from your session's working
directory. **Do NOT pass `project`, `workspace`, or `cwd` arguments unless
the user explicitly references a *different* project by name** (e.g. "what
did we decide in the `other-app` project?"). Phrases like "this project",
"here", "we", "our work", and "where did we leave off" all mean the
*current* project, so call tools with no scoping args.

This default assumes the MCP client can identify the current agent
session. Static MCP clients in parallel sessions for the same user cannot
forward the real agent session id automatically; pass explicit
`workspace` + `project` / `scopes`, or use a session-aware bridge that
forwards the lifecycle-hook session id on MCP calls.

**Lifecycle hooks already capture every prompt and tool call
automatically.** Do not manually write routine notes. Only write durable
memory when the user explicitly asks to remember or annotate something
permanently.

### Use the installed ai-memory Agent Skills

Detailed tool-routing guidance lives in the installed ai-memory Agent
Skills. When a task matches an installed ai-memory Agent Skill, load and
follow that skill before calling ai-memory tools. The skills cover memory
retrieval, handoffs, durable pages, learning maintenance, and routing
install or refresh work.

### When you write a project rule, write it here

If you're about to write a durable project rule ("always X", "never
Y", "all PRs must ..."), write it in the project's canonical agent instruction file.
Many projects use CLAUDE.md for Claude Code and
AGENTS.md for Codex / OpenCode / Cursor / Gemini CLI, but if the project
says one file is canonical, use that file.

### Refreshing this snippet

This block is maintained by ai-memory. Two ways to refresh it with the
latest binary's recommended copy:

- **From the agent** (no terminal needed): ask "refresh the ai-memory
  routing in this project". The agent calls `memory_install_self_routing`,
  picks the right filename for itself (Claude Code -> `CLAUDE.md`; Codex /
  OpenCode / Cursor / Gemini -> `AGENTS.md`), uses its Write / Edit tool
  to replace or append the returned `markered_block` while preserving
  non-ai-memory user content, then writes or updates each returned
  `managed_skills` item under the selected skill root from `target_hints`
  using its `relative_path`.
- **From the CLI**: `ai-memory install-instructions` (defaults to
  `CLAUDE.md`; pass `--target AGENTS.md` for non-Claude agents or projects
  that use `AGENTS.md` as the canonical instruction file).

Both are idempotent: re-runs replace the block bracketed by
`<!-- ai-memory:start -->` / `<!-- ai-memory:end -->` markers without
disturbing the rest of the file.
<!-- ai-memory:end -->
