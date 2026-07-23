# Roadmap de páginas, dialogs e componentes — chronos-app

- Status: levantamento do estado real do frontend, para orientar a
  integração com os casos de uso já expostos pelo backend.
- Atualizado em: 2026-07-23 (Seção 1 implementada de ponta a ponta).
- Fonte: leitura direta do código em `chronos-app/src` (rotas, páginas,
  componentes, camada `src/api`), cruzada com
  `docs/backlog/roadmap-casos-de-uso.md`.

Formato: `ordem - item (tipo) - status`.

## Seção 1 — Páginas, dialogs e componentes do MVP

1 - Autenticação: páginas de login e registro (`LoginPage`, `RegisterPage`,
`LoginForm`, `RegisterForm`) - Concluído (integradas a
`POST /v1/autenticacao/login` e `/register`, com validação `zod` e
tratamento de erro/loading)

2 - Sessão e proteção de rotas (`AuthProvider`, `ProtectedRoute`,
`PublicOnlyRoute`, `OnboardingRoute`, `CarregandoSessao`) - Concluído
(token em `localStorage`, redireciona conforme
autenticado/com organização/sem organização)

3 - UC01 Onboard organização: página e formulário (`OnboardingPage`,
`OnboardingForm`) - Concluído (integrado a `POST /v1/organizacoes`)

4 - Shell autenticado (`AppLayout`, `AppSidebar`, header com menu de perfil
e ação de sair) - Concluído (navegação real; sidebar responsiva com drawer
mobile)

5 - UC03 Gerir serviço: página `ServicosPage` - Concluído (`src/api/servicos.ts`
+ `ServicosLista` em tabela; estados de carregando/vazio/erro compartilhados)

6 - UC03 Gerir serviço: dialog de criar/editar serviço - Concluído
(`ServicoDialog`, react-hook-form + zod, mesmo padrão do `OnboardingForm`)

7 - UC02 Configurar disponibilidade: página `DisponibilidadePage` - Concluído
(`src/api/disponibilidades.ts` + `DisponibilidadeLista`, remoção com
confirmação via `alert-dialog`)

8 - UC02 Configurar disponibilidade: dialog de criar/editar janela semanal
- Concluído (`DisponibilidadeDialog`)

9 - UC07 Consultar agenda: página `DashboardPage` ("Agenda") - Concluído
(agenda diária real via `src/api/agendamentos.ts`, navegação por dia
anterior/próximo/hoje e calendário popover em `AgendaDataSeletor`)

10 - UC07 Consultar agenda: componente de calendário/grade diária ou
semanal - Concluído (`AgendaDiaGrade`; escopo do MVP cobre a visão diária —
a semanal fica para uma iteração futura, o componente se estende
trivialmente a ela quando houver demanda)

11 - UC04 Criar agendamento: dialog de criação (serviço, profissional,
pessoa atendida, horário) - Concluído (`AgendamentoDialog`; profissional é
implícito via `useProfissionalAtual`, endereço exigido só quando o serviço
é domiciliar)

12 - UC05 Reagendar/editar: dialog de reagendamento - Concluído
(`ReagendarDialog`; serviço/profissional somente-leitura, imutáveis por
regra de negócio)

13 - UC06 Confirmar/cancelar: ação ou dialog de confirmação/cancelamento -
Concluído (`AgendaPeriodoActions`: confirmar inline, cancelar via
`alert-dialog` de confirmação, feedback por toast)

### Observação

Todos os 7 casos de uso de negócio (UC01–UC07) têm página integrada ponta
a ponta, com testes unitários (Vitest + Testing Library) e E2E (Playwright,
stack real contra Postgres isolado — `chronos-app/e2e/`). A implementação
revelou que o backend, apesar de marcado "Concluído" no roadmap de casos de
uso, só cobria o lado de escrita de UC02/UC03 (sem endpoint de listagem) e
tinha dois bugs que impediam UC04/UC07 de funcionar via HTTP real contra
Postgres (owned entities com discriminador nunca atribuído e `DateTime.Kind`
incompatível com `timestamptz`) — ver `docs/backlog/roadmap-casos-de-uso.md`
para o detalhe do que foi fechado no backend.

## Seção 2 — Demais páginas, dialogs e componentes

1 - Página `ConfiguracoesPage` (perfil operacional: endereço/fuso horário
da organização) - Não implementada (texto placeholder; backend já expõe
leitura — `enderecoPrestador`/`fusoHorario` em `GET /v1/organizacoes/atual`
— e escrita — `PUT /v1/organizacoes/perfil-operacional` —, faltando só a
página)

2 - Fluxo de refresh token - Não implementado (token é armazenado mas
nunca usado; um 401 apenas desloga o usuário)

3 - Fluxo de confirmação de e-mail - Não implementado (sem tela, sem
chamada, embora o Identity possa expor o endpoint)

4 - Fluxo de autenticação em duas etapas (2FA) - Não implementado

5 - Fluxo de recuperação/redefinição de senha - Não implementado

6 - Design system: tokens de cor documentados com validação de contraste
WCAG (`src/design-system`) - Concluído

7 - Componentes shadcn/ui com Storybook (`alert`, `badge`, `button`,
`card`, `checkbox`, `form`, `input`, `label`, `password-input`,
`radio-group`, `select`, `separator`, `textarea`) - Concluído (12
componentes documentados)

8 - `dropdown-menu` (usado no menu de perfil do header) - Parcial
(funcional, mas é o único componente de `src/components/ui` sem
`.stories.tsx`)

9 - Componentes shadcn/ui necessários para o MVP: `dialog`, `alert-dialog`,
`table`, `calendar`, `popover`, `sonner`, `switch` - Concluído (7
componentes documentados, mesmo padrão do item 7; `date-picker` não é um
componente próprio do shadcn — implementado como composição local de
`popover`+`calendar` em `AgendaDataSeletor`; `sheet`/`tabs` seguem sem uso
no MVP)

10 - Landing page pública - Não implementada (a rota raiz `/` já é a área
logada; não há página de marketing/apresentação do produto)
