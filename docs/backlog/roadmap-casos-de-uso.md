# Roadmap de casos de uso — Chronos Agenda (backend)

- Status: levantamento do estado real do backend, para orientar a integração
  com `chronos-app`.
- Atualizado em: 2026-07-23 (gap de leitura de UC02/UC03/UC04/UC07 fechado
  durante a implementação da Seção 1 do roadmap de páginas/componentes;
  `GET /v1/organizacoes/atual` ganhou `possuiDisponibilidade`/`possuiServico`
  para sustentar o wizard de onboarding de 4 etapas no `chronos-app`).
- Fonte: leitura direta do código em `chronos-agenda/src` e
  `chronos-agenda/tests` (160/160 testes passando), cruzada com
  `docs/backlog/dominio.md` e `docs/backlog/plano-implementacao-mvp.md`.

Formato: `ordem - caso de uso - status`.

Ver `docs/backlog/roadmap-paginas-componentes-webapp.md` para o roadmap
equivalente de páginas, dialogs e componentes do `chronos-app`.

## Seção 1 — Casos de uso do MVP (`docs/backlog/dominio.md`)

1 - UC01 Onboard organização - Concluído (vínculo usuário↔organização
registra o papel de proprietário via `PapelMembroOrganizacao`, atribuído a
quem faz o onboarding)

2 - UC02 Configurar disponibilidade - Concluído (escrita e leitura: `GET
/v1/disponibilidades/` lista as janelas configuradas, necessário para o
frontend renderizar `DisponibilidadePage`)

3 - UC03 Gerir serviço - Concluído (escrita e leitura: `GET /v1/servicos/`
lista o catálogo, necessário para `ServicosPage` e para o seletor de
serviço do dialog de agendamento)

4 - UC04 Criar agendamento - Concluído (`PUT /v1/organizacoes/perfil-operacional`
expõe `Organizacao.ConfigurarPerfilOperacional`, destravando a criação em
produção; ver também os dois bugs corrigidos abaixo, que impediam esse
destravamento de funcionar de fato contra Postgres)

5 - UC05 Reagendar/editar agendamento - Concluído (mesmo destravamento do UC04)

6 - UC06 Confirmar/cancelar agendamento - Concluído

7 - UC07 Consultar agenda diária/semanal - Concluído (mesmo destravamento do
UC04; projeção agora também traz `ServicoId`, `NomeServico`,
`NomePessoaAtendida`, `TipoPessoaAtendida` e `EnderecoPessoaAtendida` por
período ocupado, para a tela de agenda não precisar de uma segunda consulta
por agendamento)

### Bloqueador crítico — resolvido

`PUT /v1/organizacoes/perfil-operacional` (`ConfigurarPerfilOperacionalEndpoint`)
expõe `Organizacao.ConfigurarPerfilOperacional(endereco, fusoHorario)` à
produção: recebe endereço do prestador (opcional) e fuso horário IANA,
convertendo `EnderecoAtendimentoInvalidoException`/`FusoHorarioInvalidoException`
em `Organizacao.EnderecoInvalido`/`Organizacao.FusoHorarioInvalido` (Result
Pattern). Com a organização corrente configurando seu fuso horário, UC04,
UC05 e UC07 deixam de falhar com 409 em produção.

### Dois bugs críticos encontrados e corrigidos ao validar contra Postgres real

Os testes de aplicação usam fakes em memória e nunca teriam pego estes
dois problemas, só visíveis rodando a Api de verdade contra Postgres (o que
a suíte E2E do `chronos-app` agora cobre permanentemente):

1. **Perfil operacional nunca era lido de volta.** `Organizacao.EnderecoPrestador`,
   `Organizacao.FusoHorario` e `Agendamento.Local.Endereco` usavam um
   discriminador sombra (`_discriminator`) para owned types opcionais, mas
   nada nunca atribuía seu valor — toda consulta subsequente devolvia esses
   campos nulos, fazendo UC04/UC05/UC07 falharem com 409
   `Agendamento.PerfilOperacionalNaoConfigurado` mesmo depois de configurado.
   Cada um desses tipos tem uma única propriedade escalar (sem ambiguidade
   real entre "ausente" e "presente com tudo nulo"), então o discriminador
   era desnecessário; removido (migration
   `RemoverDiscriminadoresDeOwnedEntities`), restaurando a detecção padrão
   do EF Core.
2. **500 em toda consulta de agenda.** `ProjetorDeAgenda.IntervaloDeConsultaUtc`
   construía `DateTime` via `DateOnly.ToDateTime` (`Kind=Unspecified`) e
   passava para uma coluna `timestamptz`; o Npgsql rejeita isso. Corrigido
   com `DateTime.SpecifyKind(..., DateTimeKind.Utc)`.

## Seção 2 — Demais funcionalidades e casos de uso

1 - Autenticação (registro, login, refresh token, confirmação de e-mail, 2FA)
via bundle nativo do ASP.NET Core Identity, sob `/v1/autenticacao/*` -
Concluído (backend; rota aberta, sem autenticação)

2 - Resolução de contexto/tenant por requisição (middleware que resolve a
organização corrente do usuário autenticado, base da RN01) - Concluído

3 - Swagger/OpenAPI com esquema Bearer (ambiente de desenvolvimento) -
Concluído

4 - CORS configurável para o frontend Vite - Concluído

5 - Tratamento global de exceções e `ProblemDetails` padronizado para erros
esperados e inesperados - Concluído

6 - Testes de integração de endpoints HTTP (pipeline completo com
autenticação) - Não iniciado (os 160 testes cobrem domínio e aplicação com
fakes; a Api só tem 8 testes unitários de mapeamento Result→HTTP; a suíte
E2E do `chronos-app` — `chronos-app/e2e/` — exercita a Api real contra
Postgres via HTTP, mas a partir do frontend, não substituindo uma suíte de
integração própria do backend com xunit)

7 - Health check - Concluído (`GET /health`, adicionado para o `webServer`
do Playwright saber quando a Api subiu; sem verificação de dependências
como o banco)

8 - Papéis/permissões (proprietário vs. membro) na organização - Parcial
(`PapelMembroOrganizacao` é persistido no vínculo desde o onboarding, mas
nenhum endpoint ainda restringe uma ação por papel — falta o caso de uso de
autorização que consuma `IMembroOrganizacaoRepositorio.BuscarPapelDoUsuarioAsync`)

9 - Garantia transacional contra sobreposição de agendamentos sob
concorrência real no PostgreSQL - Não iniciado (ADR pendente #1)

10 - Pagamentos, sinal, no-show, fluxo de caixa, comissões - Não iniciado
(fora do escopo do MVP por decisão de produto)

11 - Pacotes, créditos e planos de sessões - Não iniciado (fora do MVP)

12 - Lembretes por e-mail, SMS ou WhatsApp e jobs em segundo plano - Não
iniciado (fora do MVP)

13 - Portal ou widget de autoagendamento do cliente - Não iniciado (fora do
MVP)

14 - Múltiplos recursos por reserva, recorrência de agendamentos e lista de
espera - Não iniciado (fora do MVP)

15 - Precificação avançada, impostos, nota fiscal e integração contábil -
Não iniciado (fora do MVP)
