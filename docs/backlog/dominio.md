# Domínio do Chronos

- Status: backlog de produto e domínio
- Atualizado em: 2026-07-23
- Escopo: Chronos Agenda

## Visão e fronteira do domínio

O Chronos é um produto B2B de gestão de agendamentos para profissionais
individuais e pequenos negócios. Seu subdomínio central é **Booking**:
transformar a capacidade de tempo de um profissional em reservas confiáveis,
respeitando o negócio ao qual ele pertence, o serviço vendido, a pessoa
atendida e a modalidade de execução.

O sistema deve permitir que cada organização configure seu catálogo e sua
agenda, e que opere o ciclo de vida de seus atendimentos sem expor ou misturar
dados de outras organizações. Uma reserva representa um compromisso comercial
e operacional; não é, por si só, um recebimento, uma nota fiscal ou um vínculo
de autenticação.

O foco inicial são atendimentos prestados por uma pessoa. A modelagem, porém,
deve continuar válida quando uma organização tiver mais de um profissional.
Isso explica por que serviço, disponibilidade e agendamento pertencem à
organização e também referenciam o profissional responsável.

## Linguagem ubíqua

| Termo | Significado no Chronos |
| --- | --- |
| Organização | Negócio/tenant que delimita propriedade, acesso e isolamento dos dados. |
| Profissional | Pessoa que presta o serviço e cuja agenda é ocupada. |
| Serviço | Oferta comercial de um profissional, com nome, duração padrão, preço e modalidade. |
| Disponibilidade semanal | Janela recorrente de atendimento de um profissional em um dia da semana. Várias janelas podem compor o mesmo dia. |
| Agendamento | Reserva de um intervalo de tempo para um serviço, atendido por um profissional. |
| Pessoa atendida | Quem recebe o serviço; no MVP é identificada no próprio agendamento, com nome e papel (cliente, paciente, aluno ou outro). |
| Período | Intervalo fechado no início e aberto no fim para efeito de conflito: `[início, fim)`, sempre em UTC. Assim, dois atendimentos contíguos não conflitam. |
| Modalidade / tipo de atendimento | `Online`, `Domiciliar` ou `NoEnderecoDoPrestador`; determina a semântica do local. |
| Local de atendimento | Registro preservado na reserva: sem endereço físico para online; endereço da pessoa atendida para domiciliar; endereço do prestador para atendimento no negócio. |

## Modelo de domínio atual

O núcleo de domínio já modela as seguintes entidades e objetos de valor, todos
com identificadores `Guid`, auditoria em UTC e eventos de criação quando
aplicável.

| Elemento | Estado atual e responsabilidade |
| --- | --- |
| `Organizacao` | Limite do tenant, com nome, possibilidade de renomeação e perfil operacional opcional (endereço do prestador e fuso horário IANA) via `ConfigurarPerfilOperacional`, exposto em produção por `PUT /v1/organizacoes/perfil-operacional`. O vínculo usuário↔organização (fora do domínio, ADR 0003) registra o papel de proprietário desde o onboarding. |
| `Profissional` | Pertence a uma organização e possui nome de exibição. Pode ser renomeado. |
| `Servico` | Pertence à organização e ao profissional; possui nome, duração de 1 minuto a 12 horas, preço não negativo em reais (até duas casas) e modalidade. Pode ser atualizado. |
| `DisponibilidadeSemanal` | Pertence à organização e ao profissional; armazena dia da semana e uma `JanelaHorario` contínua, cujo fim deve ser posterior ao início. Pode ser reagendada. |
| `Agendamento` | Pertence à organização; referencia profissional e serviço; preserva pessoa atendida, período, preço cobrado, local e status. Nasce `Pendente` e pode ser confirmado ou cancelado. |
| `PessoaAtendida` | Snapshot mínimo com nome normalizado de até 120 caracteres e tipo de pessoa. Não há cadastro reutilizável de cliente. |
| `PeriodoAgendamento` | Exige início e fim em UTC, fim estritamente posterior ao início e expõe a duração calculada. |
| `LocalAtendimento` | Garante que online não tenha endereço e que as duas modalidades físicas tenham um endereço textual não vazio. |

O estado atual também já expressa estas regras locais:

- referências de organização, profissional e serviço não podem ser vazias;
- um agendamento cancelado não pode ser confirmado, alterado ou cancelado de
  novo;
- apenas um agendamento `Pendente` pode ser confirmado;
- dois agendamentos ativos do mesmo profissional conflitam se seus períodos se
  sobrepõem; um cancelado não ocupa a agenda;
- a auditoria não pode retroceder e recebe o horário por um provedor explícito.

`Application`, `Infrastructure` e a Api já implementam casos de uso,
persistência (EF Core/PostgreSQL) e autenticação (ASP.NET Core Identity) para
UC01–UC07 de ponta a ponta, com endpoints expostos e testados. O vínculo
usuário↔organização registra o papel de proprietário desde o onboarding
(UC01), e `PUT /v1/organizacoes/perfil-operacional` expõe
`Organizacao.ConfigurarPerfilOperacional` em produção, destravando UC04, UC05
e UC07 (antes bloqueados, pois toda criação, reagendamento ou consulta de
agenda falhava sem endereço/fuso horário configurados). Ver
`docs/backlog/roadmap-casos-de-uso.md` para o detalhamento por caso de uso.

## Invariantes-alvo do Booking

As invariantes a seguir definem o comportamento final desejado para o núcleo.
As já implementadas estão identificadas para separar contrato de intenção.

| ID | Regra | Situação |
| --- | --- | --- |
| RN01 | Toda leitura e escrita é restrita explicitamente à organização corrente; referências cruzadas entre tenants são rejeitadas. | Implementada: `ResolucaoContextoUsuarioMiddleware` resolve a organização corrente do usuário autenticado (`membros_organizacao`) e todos os handlers restringem consultas e escritas a ela. O vínculo registra o papel de proprietário (`PapelMembroOrganizacao`), mas nenhum endpoint ainda restringe uma ação por papel. |
| RN02 | Um agendamento ativo não se sobrepõe a outro agendamento ativo do mesmo profissional. Intervalos que apenas encostam são válidos. | Parcial: cálculo, consulta de conflito (`BuscarAtivosSobrepostosAsync`) e persistência implementados na aplicação; falta garantia transacional no PostgreSQL contra concorrência real (ADR pendente #1). |
| RN03 | O período é armazenado em UTC; o fim é posterior ao início; a duração é derivada do período. | Implementada no domínio. |
| RN04 | Ao criar ou reagendar, o profissional, o serviço e a disponibilidade consultados pertencem à organização corrente; o serviço pertence ao profissional escolhido. | Implementada: `AgendamentoPreparador` resolve as referências dentro da organização corrente e valida o vínculo serviço↔profissional. |
| RN05 | O período reservado é calculado a partir do início e da duração vigente do serviço. Alteração posterior do catálogo não altera reservas existentes. Exceções manuais de duração, se permitidas, devem ser explícitas e auditáveis. | Implementada: `PeriodoAgendamento.APartirDaDuracao` calcula o fim a partir da duração vigente; snapshot `NomeServicoContratado` preserva o nome contratado. Duração manual justificada segue pendente (ADR pendente #5). |
| RN06 | A modalidade e o local do agendamento devem ser compatíveis com o serviço: online não exige endereço físico; domiciliar exige endereço da pessoa atendida; no endereço do prestador usa endereço configurado da organização. | Implementada: `VerificadorCompatibilidadeLocal` compara modalidade e local na aplicação, e `PUT /v1/organizacoes/perfil-operacional` permite configurar o endereço da organização em produção. |
| RN07 | Um agendamento novo ou movido precisa estar dentro de uma janela de disponibilidade semanal do profissional, considerada no fuso horário da organização. Exceções devem ser uma capacidade explícita, não um efeito colateral. | Implementada: `VerificadorDisponibilidade` valida a janela no fuso da organização, configurável em produção pelo mesmo endpoint de perfil operacional da RN06. |
| RN08 | Estados válidos são `Pendente`, `Confirmado` e `Cancelado`. Cancelamento libera o horário; alteração e confirmação após cancelamento são proibidas. | Implementada no domínio. |
| RN09 | Exclusão não pode apagar silenciosamente histórico operacional. A política será retenção/arquivamento ou exclusão lógica, definida antes de expor a operação. | Pendente; exclusão não está modelada. |

## Escopo do MVP e backlog priorizado

### Marco 1 — tornar o domínio operável (concluído)

1. ✅ Caso de uso de onboarding implementado: autentica o usuário (ASP.NET
   Core Identity), cria a organização, o primeiro profissional e registra o
   papel de proprietário no vínculo usuário↔organização.
2. ✅ Perfil operacional da organização (endereço do prestador e fuso horário
   IANA) implementado no domínio via `Organizacao.ConfigurarPerfilOperacional`
   e exposto em produção por `PUT /v1/organizacoes/perfil-operacional`.
3. ✅ Casos de uso e repositórios explícitos implementados para organização,
   profissional, serviço, disponibilidade e agendamento, sempre recebendo a
   organização corrente na fronteira da aplicação (`IContextoUsuario`).
4. ✅ Endpoints autenticados, validação de entrada e Result Pattern
   implementados para falhas esperadas; exceções de domínio são convertidas em
   `ProblemDetails` na fronteira da Api.
5. ✅ Entidades e auditoria persistidas no PostgreSQL/EF Core, seguindo os
   ADRs: tabelas plurais em `snake_case` e sem filtro global de tenant.

### Marco 2 — reserva correta e resistente a concorrência (lógica concluída; falta garantia transacional)

1. ✅ Criação do agendamento a partir de `servicoId` e horário inicial
   implementada; `AgendamentoPreparador` busca o serviço autorizado e calcula
   o fim com sua duração. Ajuste manual de duração segue fora de escopo (ADR
   pendente #5).
2. ✅ Coerência entre serviço, profissional, modalidade e local validada por
   `VerificadorCompatibilidadeLocal`. A cópia do endereço da organização para
   atendimento no prestador depende do perfil operacional estar configurado
   (ver Marco 1, item 2).
3. ⚠️ Disponibilidade semanal já é consultada no fuso da organização
   (`VerificadorDisponibilidade`); bloqueios pontuais, folgas, feriados e
   encaixes manuais seguem fora do escopo do MVP.
4. ⚠️ Sobreposição impedida na camada de aplicação
   (`BuscarAtivosSobrepostosAsync`); a garantia transacional no PostgreSQL
   contra duas requisições simultâneas segue pendente e exige ADR específico
   antes da implementação (ADR pendente #1).
5. ✅ Atualização/reagendamento implementado (`ReagendarAgendamentoHandler`),
   reaproveitando a validação da criação e desconsiderando o próprio
   agendamento ao procurar conflito.
6. ✅ Confirmação, cancelamento e consulta da agenda diária/semanal
   implementados, distinguindo reservas pendentes, confirmadas e canceladas.

### Marco 3 — experiência de operação

1. Gestão completa de catálogo e expediente, inclusive várias janelas no mesmo
   dia e visualização de agenda com ocupações e espaços livres.
2. Definir a política de exclusão, arquivamento e retenção de agendamentos,
   considerando auditoria e dados pessoais.
3. Evoluir `PessoaAtendida` para uma entidade `Cliente` somente quando houver
   necessidade real de reutilizar contatos, histórico, preferências ou
   autoagendamento. Enquanto isso, o snapshot no agendamento evita criar um
   cadastro obrigatório prematuramente.
4. Definir tratamento de dados pessoais: minimização, autorização, retenção,
   exportação e exclusão conforme as obrigações aplicáveis.

## Casos de uso do MVP

Ver `docs/backlog/roadmap-casos-de-uso.md` para o roadmap ordenado com status
detalhado por caso de uso.

| ID | Caso de uso | Resultado esperado | Situação atual |
| --- | --- | --- | --- |
| UC01 | Onboard organização | Usuário autenticado passa a operar uma organização e seu profissional inicial, isolados de outros tenants. | Concluído — o vínculo usuário↔organização registra o papel de proprietário. |
| UC02 | Configurar disponibilidade | Profissional cria, altera e remove janelas semanais; a agenda passa a usar essas janelas para oferta de horários. | Concluído. |
| UC03 | Gerir serviço | Profissional cria e altera seus serviços com duração, preço e modalidade. Alterações não reescrevem reservas existentes. | Concluído. |
| UC04 | Criar agendamento | O sistema recebe serviço, profissional, pessoa atendida e horário inicial; calcula o período, resolve o local e persiste somente se todas as regras forem atendidas. | Concluído — `PUT /v1/organizacoes/perfil-operacional` expõe a configuração exigida em produção. |
| UC05 | Reagendar/editar | O sistema atualiza dados permitidos, revalida disponibilidade e conflito, mantendo a trilha de auditoria. | Concluído. |
| UC06 | Confirmar/cancelar | O status muda somente em transições permitidas; o cancelamento libera o período para novos agendamentos. | Concluído. |
| UC07 | Consultar agenda | Profissional visualiza agenda diária e semanal da própria organização, com ocupações e disponibilidade calculada. | Concluído. |

## Critérios de aceite essenciais

Para UC04 e UC05, o sistema deve:

- rejeitar profissional ou serviço inexistente, de outra organização ou não
  relacionado entre si;
- converter a intenção de horário no fuso da organização para UTC antes de
  persistir, lidando explicitamente com horários ambíguos ou inexistentes em
  mudanças de horário de verão;
- calcular `fim = início + duração do serviço` no fluxo padrão e exibir a
  duração derivada;
- exigir endereço no atendimento domiciliar, copiar o endereço configurado para
  atendimento no prestador e não exigir endereço físico para online; um link de
  reunião, se suportado, será um campo próprio e não um endereço;
- rejeitar período fora da disponibilidade ou sobreposto a reserva ativa do
  mesmo profissional, inclusive sob chamadas concorrentes;
- preservar no agendamento os dados comerciais e operacionais aplicados àquela
  reserva, para que mudanças no catálogo não alterem o histórico.

Para UC07, a agenda deve usar o fuso da organização para exibição e mostrar
como ocupados os agendamentos pendentes e confirmados; cancelados não ocupam
capacidade, mas continuam acessíveis no histórico conforme a política de
retenção.

## Fora do escopo do MVP, mas compatível com a visão final

- pagamentos, sinal, no-show, recebimentos, fluxo de caixa, comissões e split;
- pacotes, créditos e planos de sessões;
- lembretes por e-mail, SMS ou WhatsApp e jobs em segundo plano;
- portal ou widget de autoagendamento do cliente;
- múltiplos recursos simultâneos por reserva (salas, equipamentos ou equipes),
  recorrência de agendamentos e lista de espera;
- regras avançadas de preço, impostos, nota fiscal e integração contábil.

Essas capacidades não devem ser antecipadas no modelo atual. Quando entrarem
no escopo, precisam preservar as invariantes de tenant, histórico e capacidade
do profissional descritas neste documento.

## Decisões que exigem ADR antes da implementação

1. Garantia transacional de não sobreposição no PostgreSQL.
2. Resolução de tenant, associação entre identidade e organização, e regras de
   autorização.
3. Política de exclusão, retenção e tratamento de dados pessoais.
4. Modelagem de fuso horário e regras para bloqueios, feriados e exceções de
   disponibilidade.
5. Política de duração manual e de snapshots comerciais do agendamento.

Este documento descreve a direção do produto e organiza o backlog; ele não
substitui os ADRs aceitos nem altera silenciosamente suas decisões.
