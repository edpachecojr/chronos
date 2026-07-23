# Roadmap de casos de uso — Chronos Agenda (backend)

- Status: levantamento do estado real do backend, para orientar a integração
  com `chronos-app`.
- Atualizado em: 2026-07-23.
- Fonte: leitura direta do código em `chronos-agenda/src` e
  `chronos-agenda/tests` (152/152 testes passando), cruzada com
  `docs/backlog/dominio.md` e `docs/backlog/plano-implementacao-mvp.md`.

Formato: `ordem - caso de uso - status`.

Ver `docs/backlog/roadmap-paginas-componentes-webapp.md` para o roadmap
equivalente de páginas, dialogs e componentes do `chronos-app`.

## Seção 1 — Casos de uso do MVP (`docs/backlog/dominio.md`)

1 - UC01 Onboard organização - Concluído (vínculo usuário↔organização
registra o papel de proprietário via `PapelMembroOrganizacao`, atribuído a
quem faz o onboarding)

2 - UC02 Configurar disponibilidade - Concluído

3 - UC03 Gerir serviço - Concluído

4 - UC04 Criar agendamento - Concluído (`PUT /v1/organizacoes/perfil-operacional`
expõe `Organizacao.ConfigurarPerfilOperacional`, destravando a criação em
produção)

5 - UC05 Reagendar/editar agendamento - Concluído (mesmo destravamento do UC04)

6 - UC06 Confirmar/cancelar agendamento - Concluído

7 - UC07 Consultar agenda diária/semanal - Concluído (mesmo destravamento do UC04)

### Bloqueador crítico — resolvido

`PUT /v1/organizacoes/perfil-operacional` (`ConfigurarPerfilOperacionalEndpoint`)
expõe `Organizacao.ConfigurarPerfilOperacional(endereco, fusoHorario)` à
produção: recebe endereço do prestador (opcional) e fuso horário IANA,
convertendo `EnderecoAtendimentoInvalidoException`/`FusoHorarioInvalidoException`
em `Organizacao.EnderecoInvalido`/`Organizacao.FusoHorarioInvalido` (Result
Pattern). Com a organização corrente configurando seu fuso horário, UC04,
UC05 e UC07 deixam de falhar com 409 em produção.

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
autenticação) - Não iniciado (os 152 testes cobrem domínio e aplicação com
fakes; a Api só tem 8 testes unitários de mapeamento Result→HTTP)

7 - Health check - Não iniciado

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
