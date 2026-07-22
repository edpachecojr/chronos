# Plano de implementação do MVP — domínio e aplicação

- Status: backlog de produto e domínio
- Atualizado em: 2026-07-22 (Fase B do domínio concluída: RN05, RN06, conflito
  de disponibilidade e RN07; Fase 0 da aplicação concluída: Result Pattern
  reaproveitado do domínio, portas de repositório, unidade de trabalho e
  resolução do tenant corrente — `IMembroOrganizacaoRepositorio` e
  `ContextoUsuario`; UC01 a UC07 implementados em Application — onboard de
  organização, configurar disponibilidade, gerir serviço, criar e reagendar
  agendamento, confirmar/cancelar e consultar agenda diária/semanal;
  `FusoHorario.ConverterParaLocal` adicionado ao domínio; ADR 0005 resolve,
  para este escopo, a parte do ADR pendente #4 sobre horário ambíguo/inexistente;
  UC04 e UC05 compartilham preparação/validação via `AgendamentoPreparador`, e
  UC07 compartilha resolução de fuso e projeção diária via `ProjetorDeAgenda`,
  ambos internos à feature `Agendamentos`; item 8 do sequenciamento concluído:
  persistência EF Core/PostgreSQL, repositórios concretos, unidade de trabalho,
  `ProvedorDataHoraUtc`, `IServicoAutenticacao`/`ServicoAutenticacao` sobre o
  ASP.NET Core Identity e composição de `IContextoUsuario` na Api — ADR 0006
  resolve o ADR pendente #2 quanto ao mecanismo de sessão (tokens de portador
  do Identity); endpoints da Api seguem fora de escopo)
- Escopo: Chronos Agenda
- Base: `docs/backlog/dominio.md`, ADR 0001, ADR 0002, ADR 0003, ADR 0005 e ADR 0006

Este documento traduz os casos de uso do MVP em (1) o que o domínio atual já
cobre, (2) o que falta no domínio para sustentar cada caso de uso, (3) um
plano de implementação do domínio restante e (4) um plano completo dos casos
de uso na camada de aplicação. Ele não substitui `dominio.md` nem os ADRs;
detalha a execução do backlog já priorizado lá.

## 1. Cobertura atual por caso de uso

| UC | Caso de uso | Domínio hoje | Gap de domínio |
| --- | --- | --- | --- |
| UC01 | Onboard organização | `Organizacao.Criar`/`Renomear`/`ConfigurarPerfilOperacional`, `Profissional.Criar`/`Renomear` | Sem gap de domínio. O vínculo usuário↔organização (proprietário) não é modelado no domínio (ADR 0003); falta a associação em Application/Infra e a implementação de `IContextoUsuario` (ver Fase A item 4 e **[ADR pendente #2]**) |
| UC02 | Configurar disponibilidade | `DisponibilidadeSemanal.Criar`/`Reagendar`, `JanelaHorario` valida início<fim, `DisponibilidadeSemanal.ConflitaCom`/`JanelaHorario.Sobrepoe` (Fase B item 8) | Sem gap de domínio. A checagem de sobreposição existe no domínio; falta só a aplicação consultá-la ao criar/alterar (ver UC02 na seção 3) |
| UC03 | Gerir serviço | `Servico.Criar`/`Atualizar` completo, `DuracaoServico`, `PrecoServico`, `NomeServico` validados | Nenhum gap. O snapshot comercial já existe em `Agendamento` (Fase B item 7) |
| UC04 | Criar agendamento | `Agendamento.Criar` (com `NomeServicoContratado`/`DuracaoReservada`), `PeriodoAgendamento.APartirDaDuracao` (RN05), `VerificadorCompatibilidadeLocal` (RN06) e `VerificadorDisponibilidade` (RN07) — Fase B itens 5–9 | Sem gap de domínio. Falta apenas orquestrar essas peças e validar serviço×profissional×organização (RN04) na aplicação (ver UC04 na seção 3) |
| UC05 | Reagendar/editar | Mesmas peças de domínio de UC04 | Sem gap de domínio (mesma orquestração de UC04 na aplicação) |
| UC06 | Confirmar/cancelar | `Confirmar`/`Cancelar` com as transições de `StatusAgendamento` corretas | Nenhum. Caso de uso já suportado integralmente pelo domínio |
| UC07 | Consultar agenda | `Agendamento.ConflitaCom`, `Status`, `Periodo` dão a base para ocupação | Nenhum gap de domínio; é puramente leitura/projeção na aplicação |

`Application`, `Infrastructure` e `Api` não têm nenhum código de produto ainda
(apenas `.csproj` e o `Program.cs` padrão) — todo o plano da seção 3 parte do
zero.

## 2. Plano de implementação — camada de domínio

Ordenado por dependência. Itens marcados **[ADR pendente]** não devem ser
implementados antes da decisão correspondente (seção "Decisões que exigem
ADR" de `dominio.md`).

### Fase A — Perfil operacional da organização (destrava UC01, UC04)

1. Mover `EnderecoAtendimento` de `Agendamentos.ObjetosValor` para
   `Compartilhado.ObjetosValor`. Hoje ele mora em `Agendamentos` mas passará a
   ser usado também por `Organizacao`; manter o nome atual, só mudar o
   namespace e os `using` dependentes.
2. Criar `FusoHorario` (objeto de valor em `Organizacoes.ObjetosValor`),
   validando o identificador IANA com `TimeZoneInfo.FindSystemTimeZoneById` e
   lançando uma exceção dedicada (`FusoHorarioInvalidoException`) para IDs
   inválidos.
3. Estender `Organizacao` com `EnderecoPrestador` (`EnderecoAtendimento?`) e
   `FusoHorario`, mais um método `ConfigurarPerfilOperacional(endereco,
   fusoHorario, provedorDataHora)`. Enquanto não configurado, `Organizacao`
   permanece utilizável para UC02/UC03, mas UC04 com `NoEnderecoDoPrestador`
   deve falhar de forma esperada (Result Pattern na aplicação) se o endereço
   não estiver configurado.
4. **[ADR 0003]** Usuário não é modelado no domínio. Uma entidade `Usuario`
   chegou a ser criada (implementando `IPertenceOrganizacao`, com um
   `PapelUsuario` para distinguir proprietário de outros papéis) e foi
   removida: "usuário" é um conceito de autenticação/acesso (ASP.NET Core
   Identity, ADR 0001), não um papel de negócio reconhecido pela linguagem
   ubíqua do domínio. Os papéis reais já têm ou terão representação própria
   — `Profissional` existe; uma futura pessoa atendida persistente evolui de
   `PessoaAtendida` (hoje objeto de valor em `Agendamentos`); "proprietário"
   é, por ora, uma questão de autorização, não de invariante de domínio.
   Detalhes completos do raciocínio e das consequências estão no ADR 0003.
   - O que permanece no domínio: `Compartilhado.Contratos.IContextoUsuario`
     (`Guid UsuarioId`, `Guid ObterOrganizacaoId()`). Expõe o identificador
     do usuário autenticado (a sessão já o conhece diretamente) e um método
     — não uma propriedade — para resolver a organização corrente, já que
     essa resolução pode exigir consulta a um vínculo persistido. Nenhuma
     entidade do domínio depende dessa interface; ela é a porta que
     Application vai consumir (ver Fase 0 da seção 3).
   - O que fica pendente para Application/Infrastructure: a associação
     usuário↔organização (por exemplo, uma tabela `membros_organizacao` com
     `usuario_id`, `organizacao_id` e um papel) e a implementação de
     `IContextoUsuario` (extrair o usuário autenticado e consultar esse
     vínculo). A forma exata depende do **[ADR pendente #2]** (mecanismo de
     autenticação/sessão), que não bloqueia mais nenhuma modelagem de
     domínio — só a implementação de Application/Infra.

### Fase B — Coerência de serviço, local e disponibilidade (destrava UC04, UC05) — concluída

5. ✅ `PeriodoAgendamento.APartirDaDuracao(inicioUtc, DuracaoServico duracao)`
   implementado como factory nomeada; `fim = início + duração` é sempre
   calculado da mesma forma em vez de recebido pronto do chamador (RN05).
6. ✅ Checagem de compatibilidade entre modalidade e local
   (`Servico.TipoAtendimento` vs `LocalAtendimento.Tipo`) implementada em
   `Agendamentos/Servicos/VerificadorCompatibilidadeLocal` (função estática
   pura, não uma entidade nova), lançando `LocalIncompativelComServicoException`
   quando divergem. Não é chamada pelo próprio `Agendamento` — cabe à
   aplicação invocá-la antes de `Agendamento.Criar`/`Atualizar` (ver UC04
   passo 4 na seção 3), já que a entidade não guarda o `TipoAtendimento` do
   serviço, apenas o do local escolhido.
7. ✅ Snapshot comercial adicionado ao `Agendamento`: campo
   `NomeServicoContratado` (string, validado como não vazio na criação,
   imutável — `Atualizar` não o altera, pois `ServicoId` também não muda após
   a criação). `DuracaoReservada => Periodo.Duracao` expõe a duração
   reservada de forma explícita para a leitura da RN05.
   **[ADR pendente #5]** Segue pendente: se ajuste manual de duração for
   aceito, adicionar um par opcional `DuracaoManualJustificativa` só depois
   dessa decisão.
8. ✅ `JanelaHorario.Sobrepoe(outra)` e `DisponibilidadeSemanal.ConflitaCom(outra)`
   implementados, espelhando `PeriodoAgendamento.Sobrepoe` e
   `Agendamento.ConflitaCom`, impedindo duas janelas sobrepostas do mesmo
   profissional no mesmo dia da semana.
9. ✅ Verificação "período cabe na disponibilidade" (RN07) implementada em
   `Disponibilidades/Servicos/VerificadorDisponibilidade`, um serviço de
   domínio dedicado (cruza `Organizacao`, `DisponibilidadeSemanal` e
   `Agendamento`, então não incha nenhuma entidade): converte
   `PeriodoAgendamento` (UTC) para o fuso da organização via
   `FusoHorario.ConverterParaLocal` (novo método) e verifica se o intervalo
   local cabe inteiramente em uma `JanelaHorario` do dia da semana
   correspondente ao início local, lançando
   `PeriodoForaDaDisponibilidadeException` quando não cabe. Períodos cujo
   início e fim (já em horário local) caem em datas diferentes são
   rejeitados diretamente, pois nenhuma `JanelaHorario` pode representar uma
   janela que atravessa a meia-noite.
   **[ADR pendente #4] resolvido para este escopo:** a conversão feita aqui é
   sempre UTC → local, que nunca é ambígua nem inexistente (isso só ocorre no
   sentido local → UTC). Logo essa verificação não precisa — e não deve —
   escolher um offset nem lançar uma exceção de horário ambíguo; esse
   tratamento continua sendo responsabilidade da aplicação ao converter o
   horário local informado pelo usuário para UTC antes de montar o
   `PeriodoAgendamento` (UC04 passo 2 na seção 3). O ADR pendente #4 permanece
   necessário apenas para essa conversão local → UTC feita na aplicação.

Nenhuma outra alteração de domínio é necessária para fechar UC01–UC07 do MVP;
RN01 (isolamento de tenant) e RN02 (concorrência transacional) são
responsabilidade de aplicação/infraestrutura, não do domínio.

## 3. Plano de implementação — camada de aplicação

### Fase 0 — Fundamentos transversais — concluída

Antes de qualquer caso de uso:

- ✅ **Result Pattern**: em vez de um `Resultado<T>` duplicado em
  `Chronos.Agenda.Application.Compartilhado`, o `Resultado<T>` genérico foi
  adicionado ao lado do `Resultado` não genérico já existente em
  `Chronos.Agenda.Domain.Compartilhado.Resultados` (mesmo arquivo
  `Resultado.cs`). A Aplicação depende do Domínio (regra de camadas já
  vigente), então reaproveita o mesmo `Resultado`/`Resultado<T>`/`Erro` do
  domínio em vez de duplicar o padrão — os casos de uso usarão `Resultado`
  para falhas sem valor e `Resultado<T>` quando precisarem retornar um valor
  (ex.: o `Guid` criado). Catálogos de erro da aplicação seguem o mesmo
  padrão `record` por feature já usado no domínio (`AgendamentoErros`,
  `DisponibilidadeErros`), quando os casos de uso (seção seguinte) forem
  implementados. Falhas esperadas (entidade inexistente, fora de outra
  organização, conflito de agenda, fora de expediente, endereço ausente)
  retornam `Resultado`/`Resultado<T>` de erro. Exceções de domínio continuam
  reservadas a violações de invariante não tratadas pelo fluxo (ex.:
  `Guid.Empty` vindo de um bug interno) e são convertidas na fronteira da
  API, conforme ADR 0001.
- ✅ **Portas de repositório**, uma por agregado, expondo só os métodos que os
  casos de uso da seção 3 usam (sem CRUD genérico), em
  `Chronos.Agenda.Application.<Feature>.Contratos` (mesma convenção de pasta
  `Contratos` já usada em `Domain.Compartilhado.Contratos`):
  `IOrganizacaoRepositorio`, `IProfissionalRepositorio`, `IServicoRepositorio`,
  `IDisponibilidadeSemanalRepositorio`, `IAgendamentoRepositorio`. Toda busca
  recebe `OrganizacaoId` explicitamente e filtra por ele na própria
  assinatura do método (RN01, ADR 0001) — nunca via query filter global.
  Testados com fakes nomeados em `Chronos.Agenda.Application.Tests`
  (`Fakes/Fake<Agregado>Repositorio`), cobrindo isolamento de tenant e os
  filtros que os casos de uso vão exigir (dia da semana, sobreposição de
  período, exclusão de cancelados).
- ✅ **`IUnidadeDeTrabalho`**: adicionado em
  `Chronos.Agenda.Application.Compartilhado.Contratos` (`SalvarAlteracoesAsync`),
  para a atomicidade exigida por UC01 e demais casos de uso com múltiplas
  escritas.
- Pendente (não é trabalho de Application): **`IProvedorDataHora`** já existe
  no domínio; a implementação concreta `ProvedorDataHoraUtc` vive em
  `Infrastructure` (hoje só existe o fake de teste).
- ✅ **Resolução do tenant corrente (parte de Application)**: o contrato já
  existia no domínio — `Compartilhado.Contratos.IContextoUsuario`
  (`UsuarioId` + `ObterOrganizacaoId()`, ver Fase A item 4 e ADR 0003).
  Adicionados em Application:
  - `Compartilhado.Contratos.IMembroOrganizacaoRepositorio`
    (`AdicionarAsync`, `BuscarOrganizacaoIdDoUsuarioAsync`): porta para o
    vínculo persistido usuário↔organização (RN01, ADR 0003). Não modela
    papel/autorização (ex.: proprietário) ainda — só o necessário para
    resolver o tenant; entra quando um caso de uso de autorização exigir.
  - `Compartilhado.ContextoUsuario`: implementação de `IContextoUsuario` que
    apenas devolve `UsuarioId`/`OrganizacaoId` já resolvidos no construtor.
    Resolve a tensão entre `ObterOrganizacaoId()` ser síncrono (contrato
    fechado no domínio) e a consulta ao vínculo ser assíncrona: a resolução
    assíncrona acontece uma única vez por requisição, fora desta classe.
  - Testados com `FakeMembroOrganizacaoRepositorio` e teste de
    `ContextoUsuario` em `Chronos.Agenda.Application.Tests`.
  - Pendente (bloqueado por ADR): a implementação concreta de
    `IMembroOrganizacaoRepositorio` em Infrastructure (consulta a
    `membros_organizacao`) e a composição do `ContextoUsuario` por
    requisição na Api (extrair o usuário autenticado do Identity/HTTP e
    montá-lo como serviço scoped, chamando o repositório uma única vez antes
    do pipeline continuar). A forma exata dessa associação e do mecanismo de
    autenticação/sessão depende do **[ADR pendente #2]** — não a forma dos
    contratos de Application, que já estão fechados. Casos de uso recebem a
    organização corrente chamando `IContextoUsuario.ObterOrganizacaoId()`
    explicitamente (RN01), sem acoplar ao mecanismo de autenticação.

### UC01 — Onboard organização — concluída

- Comando: `CriarOrganizacaoComando(UsuarioId, Nome, NomeProfissionalInicial)`,
  em `Application.Organizacoes.CriarOrganizacao` (cada caso de uso tem sua
  própria pasta dentro da feature — ver ADR 0004). `CriarOrganizacaoHandler`
  constrói `NomeOrganizacao`/`Nome` (capturando `NomeOrganizacaoInvalidoException`/
  `NomeInvalidoException` do domínio e convertendo em `Resultado` de falha —
  `OrganizacaoErros.NomeInvalido`/`ProfissionalErros.NomeInvalido`, novos
  catálogos de erro da aplicação), monta `Organizacao.Criar` e
  `Profissional.Criar` com o `OrganizacaoId` gerado, e só então persiste os
  dois e registra o vínculo usuário↔organização
  (`IMembroOrganizacaoRepositorio.AdicionarAsync`) numa única chamada a
  `IUnidadeDeTrabalho.SalvarAlteracoesAsync` — nada é persistido se a
  validação de qualquer um dos nomes falhar.
- Retorno: `Resultado<CriarOrganizacaoResultado>` com `OrganizacaoId` e
  `ProfissionalId`.
- Portas usadas: `IOrganizacaoRepositorio.AdicionarAsync`,
  `IProfissionalRepositorio.AdicionarAsync`,
  `IMembroOrganizacaoRepositorio.AdicionarAsync`, `IUnidadeDeTrabalho` (todas
  já definidas na Fase 0).
- Testado com fakes (`FakeOrganizacaoRepositorio`, `FakeProfissionalRepositorio`,
  `FakeMembroOrganizacaoRepositorio`, novo `FakeUnidadeDeTrabalho`) cobrindo o
  caminho de sucesso e as duas falhas de nome inválido, incluindo a ausência
  de persistência quando a validação falha.
- **Pendente, não bloqueado — decisão explícita adiada, não um gap de
  implementação:** o vínculo registrado não carrega papel de proprietário,
  porque `IMembroOrganizacaoRepositorio` ainda não modela papéis (decisão da
  Fase 0, reafirmada aqui). Quando um caso de uso de autorização exigir
  distinguir proprietário de outros papéis, o contrato precisará crescer
  (ex.: parâmetro de papel em `AdicionarAsync`) e este caso de uso precisará
  passá-lo.
- **Fora do escopo de Application, bloqueado por [ADR pendente #2]:** a
  origem real do `UsuarioId` (extração do usuário autenticado do
  ASP.NET Core Identity/HTTP na Api) e a implementação concreta de
  `IMembroOrganizacaoRepositorio` em Infrastructure. `CriarOrganizacaoComando`
  recebe `UsuarioId` como um `Guid` já resolvido, então a Api pode compor este
  caso de uso assim que o mecanismo de autenticação for decidido, sem
  qualquer mudança em Application.

### UC02 — Configurar disponibilidade — concluída

- Comandos: `CriarDisponibilidadeComando`, `AlterarDisponibilidadeComando`,
  `RemoverDisponibilidadeComando`, cada um em sua própria pasta de caso de uso
  (`Application.Disponibilidades.<CasoDeUso>`, ADR 0004).
- Passos (criar/alterar): resolver a organização corrente
  (`IContextoUsuario.ObterOrganizacaoId()`, RN01) → validar que o profissional
  existe na organização → construir a `JanelaHorario` (falha esperada de
  domínio — fim não posterior ao início — convertida em
  `Disponibilidade.JanelaInvalida`, `JanelaHorarioFactory` compartilhada entre
  os dois casos de uso) → carregar as demais janelas do profissional naquele
  dia (criar) ou de todas as suas janelas (alterar, para localizar a
  disponibilidade por id, já que o repositório não expõe busca direta por id)
  → checar `ConflitaCom`/sobreposição (Fase B, item 8) → `DisponibilidadeSemanal.Criar`/
  `Reagendar` → persistir numa única `IUnidadeDeTrabalho.SalvarAlteracoesAsync`.
- Remover é uma exclusão direta via repositório; não há política de
  retenção para configuração de expediente (diferente do agendamento).
- Portas: `IDisponibilidadeSemanalRepositorio` (`BuscarPorProfissionalEDia`,
  `BuscarPorProfissional`, `Adicionar`, `Atualizar`, `Remover`),
  `IProfissionalRepositorio.BuscarPorId` (todas já definidas na Fase 0).
- Erros esperados: `Profissional.NaoEncontrado` (novo em `ProfissionalErros`,
  reaproveitado por UC02/UC03/UC04), `Disponibilidade.JanelaInvalida`,
  `Disponibilidade.JanelaSobreposta`, `Disponibilidade.NaoEncontrada` (novo
  catálogo `Application.Disponibilidades.Erros.DisponibilidadeErros`).
- Testado com fakes cobrindo sucesso, profissional inexistente, janela
  inválida, sobreposição (criar e alterar) e disponibilidade inexistente
  (alterar/remover).

### UC03 — Gerir serviço — concluída

- Comandos: `CriarServicoComando`, `AtualizarServicoComando`, cada um em sua
  própria pasta de caso de uso (`Application.Servicos.<CasoDeUso>`).
- Passos: resolver a organização corrente → validar que o profissional existe
  na organização (só em `CriarServico`; `AtualizarServico` já busca o serviço
  filtrado por organização, e `ProfissionalId` não muda após a criação) →
  construir `NomeServico`/`DuracaoServico`/`PrecoServico` (falhas de domínio
  capturadas e convertidas por `ConfiguracaoServicoFactory`, compartilhada
  entre os dois casos de uso) → `Servico.Criar`/`Atualizar` → persistir.
- Portas: `IServicoRepositorio`, `IProfissionalRepositorio.BuscarPorId`.
- Erros esperados: `Profissional.NaoEncontrado`, `Servico.NomeInvalido`,
  `Servico.DuracaoInvalida`, `Servico.PrecoInvalido` (novo catálogo
  `Application.Servicos.Erros.ServicoErros`), `Servico.NaoEncontrado`.
- Nenhuma reescrita de agendamentos existentes é necessária: o snapshot em
  `Agendamento` (Fase B item 7) já isola o histórico de mudanças futuras no
  catálogo.

### UC04 — Criar agendamento — concluída

- Comando: `CriarAgendamentoComando { ProfissionalId, ServicoId,
  NomePessoaAtendida, TipoPessoaAtendida, Inicio (DateTimeOffset),
  EnderecoPessoaAtendida? }`, em `Application.Agendamentos.CriarAgendamento`.
  `Inicio` carrega o offset explícito informado pelo cliente (ADR 0005), não
  mais um horário local "ingênuo" a ser desambiguado pela aplicação.
- Passos (`CriarAgendamentoHandler`, dividido em `PrepararAsync` — referências,
  pessoa atendida, período e local — e `ValidarRegrasAsync` — disponibilidade
  e conflito):
  1. Buscar `Profissional` e `Servico` na organização corrente
     (`IContextoUsuario.ObterOrganizacaoId()`, RN01); rejeitar se não
     existirem ou se o serviço não pertencer ao profissional informado (RN04).
     Buscar a `Organizacao` e rejeitar se o perfil operacional (fuso horário)
     não estiver configurado.
  2. Converter `Inicio` para UTC com `DateTimeOffset.UtcDateTime` — operação
     total, sem horário ambíguo/inexistente a tratar (ADR 0005).
  3. Calcular o período com `PeriodoAgendamento.APartirDaDuracao` usando a
     duração vigente do serviço (Fase B, item 5).
  4. Resolver `LocalAtendimento` a partir de `Servico.TipoAtendimento`: sem
     endereço para `Online`; endereço da pessoa atendida para `Domiciliar`
     (obrigatório na entrada, capturando `EnderecoAtendimentoInvalidoException`);
     cópia do `Organizacao.EnderecoPrestador` para `NoEnderecoDoPrestador`
     (falha esperada se a organização não tiver endereço configurado). Chama
     `VerificadorCompatibilidadeLocal.Verificar` (Fase B, item 6) como checagem
     defensiva de invariante.
  5. Verificar se o período cabe em uma janela de disponibilidade do
     profissional: converte início/fim para o fuso da organização com o novo
     `FusoHorario.ConverterParaLocal` (ADR 0005), rejeita diretamente
     (`Agendamento.PeriodoAtravessaMeiaNoite`) se início e fim locais caem em
     dias diferentes, e só então chama `VerificadorDisponibilidade.Verificar`
     (Fase B, item 9).
  6. Buscar agendamentos ativos do profissional que se sobrepõem ao período
     (`IAgendamentoRepositorio.BuscarAtivosSobrepostosAsync`) e rejeitar em
     caso de conflito (RN02) — a garantia contra concorrência real (duas
     requisições simultâneas) só fecha com o **[ADR pendente #1]** no Marco 2.
  7. `Agendamento.Criar(...)`, persistir numa única
     `IUnidadeDeTrabalho.SalvarAlteracoesAsync`; `AgendamentoCriado` já é
     lançado pela própria entidade (`Entidade.ObterEventosDominio`), a
     publicação efetiva fica para quando a Infraestrutura existir.
- Portas: `IProfissionalRepositorio`, `IServicoRepositorio`,
  `IDisponibilidadeSemanalRepositorio`, `IAgendamentoRepositorio`,
  `IOrganizacaoRepositorio` (para o endereço/fuso).
- Erros esperados: `Profissional.NaoEncontrado`, `Servico.NaoEncontrado`,
  `Agendamento.ServicoNaoPertenceAoProfissional`,
  `Agendamento.PerfilOperacionalNaoConfigurado`,
  `Agendamento.NomePessoaAtendidaInvalido`,
  `Agendamento.EnderecoObrigatorioAusente`, `Agendamento.EnderecoInvalido`,
  `Agendamento.PeriodoAtravessaMeiaNoite`, `Disponibilidade.ForaDaJanela`
  (domínio), `Agendamento.ConflitoDeAgenda` (novo catálogo
  `Application.Agendamentos.Erros.AgendamentoErros`).

### UC05 — Reagendar/editar — concluída

- Comando: `ReagendarAgendamentoComando { AgendamentoId, ProfissionalId,
  ServicoId, NomePessoaAtendida, TipoPessoaAtendida, Inicio,
  EnderecoPessoaAtendida }` — mesmos campos de UC04, em
  `Application.Agendamentos.ReagendarAgendamento`.
- A preparação (referências, RN04, pessoa atendida, período RN05, local RN06)
  e a validação (disponibilidade RN07, conflito RN02) foram extraídas de
  `CriarAgendamentoHandler` para `Agendamentos.AgendamentoPreparador`
  (`PrepararAsync`/`ValidarRegrasAsync`), reaproveitado por criar e reagendar
  — a mesma orquestração que a seção 4 já antecipava.
  `IAgendamentoRepositorio.BuscarAtivosSobrepostosAsync` ganhou o parâmetro
  `excluirAgendamentoId` para excluir o próprio agendamento da busca de
  conflito ao reagendar (`null` ao criar).
- `ReagendarAgendamentoHandler` busca o agendamento pela organização corrente
  (RN01) e rejeita, com um novo erro
  (`Agendamento.AlteracaoDeProfissionalOuServicoNaoPermitida`), qualquer
  tentativa de informar um `ProfissionalId`/`ServicoId` diferente do já
  vinculado — ambos são imutáveis em `Agendamento` após a criação, então o
  comando não poderia de fato aplicá-los. Chama `Agendamento.Atualizar(...)`
  ao final, que já retorna `Resultado` de domínio (`Agendamento.JaCancelado`
  quando o agendamento já foi cancelado) — a aplicação apenas repassa esse
  resultado, sem precisar capturar exceção.
- Portas: as mesmas de UC04, mais `IAgendamentoRepositorio.AtualizarAsync`.
- Erros esperados: os mesmos de UC04, mais `Agendamento.NaoEncontrado`,
  `Agendamento.AlteracaoDeProfissionalOuServicoNaoPermitida` e
  `Agendamento.JaCancelado` (domínio, repassado como está).

### UC06 — Confirmar/cancelar — concluída

- Comandos: `ConfirmarAgendamentoComando`, `CancelarAgendamentoComando`, cada
  um em sua própria pasta de caso de uso
  (`Application.Agendamentos.ConfirmarAgendamento`/`CancelarAgendamento`).
- Passos: buscar o agendamento filtrando pela organização corrente (RN01) →
  chamar `Confirmar`/`Cancelar` → persistir numa única
  `IUnidadeDeTrabalho.SalvarAlteracoesAsync`.
- `Agendamento.Confirmar`/`Cancelar` já retornam `Resultado` de domínio
  (`Agendamento.ConfirmacaoInvalida`, `Agendamento.JaCancelado`) em vez de
  lançar exceção — diferente do que a seção original deste documento previa
  (`ConfirmacaoAgendamentoInvalidaException`/`AgendamentoCanceladoException`),
  o domínio já foi implementado com o Result Pattern nessas transições; a
  aplicação apenas repassa o `Resultado` recebido, sem capturar exceção.
- Portas: `IAgendamentoRepositorio.BuscarPorIdAsync`/`AtualizarAsync`.
- Erros esperados: `Agendamento.NaoEncontrado` (novo, aplicação),
  `Agendamento.ConfirmacaoInvalida`/`Agendamento.JaCancelado` (domínio).

### UC07 — Consultar agenda — concluída

- Consultas: `ConsultarAgendaDiariaConsulta { ProfissionalId, Data }`,
  `ConsultarAgendaSemanalConsulta { ProfissionalId, InicioDaSemana }`, cada
  uma em sua própria pasta de caso de uso
  (`Application.Agendamentos.ConsultarAgendaDiaria`/`ConsultarAgendaSemanal`).
- A resolução do fuso horário da organização corrente (RN01) e a projeção de
  um dia (janelas de disponibilidade do dia da semana + períodos ocupados por
  agendamentos ativos) foram extraídas para `Agendamentos.ProjetorDeAgenda`,
  reaproveitado pelas duas consultas; a consulta semanal chama a projeção
  diária sete vezes a partir de `InicioDaSemana`.
- A conversão de data local para intervalo UTC de busca **não** usa
  local→UTC (ambíguo, ADR pendente #4): a projeção busca um intervalo UTC
  alargado em um dia para cada lado do dia local pedido — cobrindo qualquer
  offset possível — e filtra a data local exata de cada agendamento
  retornado convertendo UTC→local (`FusoHorario.ConverterParaLocal`, nunca
  ambíguo), a mesma direção de conversão já usada por RN07 e pelo ADR 0005.
- Cancelados não ocupam capacidade (são excluídos da projeção), mas a
  consulta de histórico com cancelados fica para a política de retenção do
  **[ADR pendente #3]** (Marco 3 — não bloqueia o MVP).
- Resultado: `AgendaDiariaResultado { Data, DiaDaSemana, JanelasDisponiveis,
  PeriodosOcupados }` (com `PeriodoOcupado { AgendamentoId, Inicio, Fim,
  Status }`, tudo já no fuso local); `AgendaSemanalResultado { Dias }` agrega
  um `AgendaDiariaResultado` por dia.
- Portas: `IAgendamentoRepositorio.BuscarPorProfissionalEPeriodoAsync`,
  `IDisponibilidadeSemanalRepositorio.BuscarPorProfissionalEDiaAsync`,
  `IOrganizacaoRepositorio.BuscarPorIdAsync`,
  `IProfissionalRepositorio.BuscarPorIdAsync`.
- Sem exceções de domínio esperadas; é leitura pura. Erros esperados apenas
  de referência: `Profissional.NaoEncontrado`, `Organizacao.NaoEncontrada`,
  `Agendamento.PerfilOperacionalNaoConfigurado` (reaproveitado de UC04, já
  que a projeção também depende do fuso horário da organização).

## 4. Sequenciamento recomendado

1. ✅ Domínio Fase A (perfil operacional) → destrava UC01 e o local
   `NoEnderecoDoPrestador` de UC04.
2. ✅ Domínio Fase B, itens 5–9 (período por duração, compatibilidade de
   local, snapshot, conflito de disponibilidade e verificação RN07)
   concluída — nenhum item ficou bloqueado por ADR; a conversão UTC→local
   usada na verificação de disponibilidade não é ambígua, então o item 9 não
   precisou esperar o **[ADR pendente #4]** (ver Fase B item 9).
3. ✅ Aplicação Fase 0 (fundamentos) — concluída.
4. ✅ Aplicação UC01 concluída. UC02, UC03 — não dependem de garantias de
   concorrência.
5. ✅ Aplicação UC04 concluída — ADR 0005 resolveu, para este escopo, a
   dependência do **[ADR pendente #4]** que bloqueava o passo 2 (conversão
   local→UTC do horário informado pelo usuário): o contrato de entrada passou
   a exigir offset explícito (`DateTimeOffset`), eliminando o cenário de
   horário ambíguo/inexistente nessa direção. UC05 (reagendar) reaproveita a
   mesma infraestrutura de UC04, extraída para `AgendamentoPreparador`
   (`PrepararAsync`, `ValidarRegrasAsync`), e não tem mais nenhuma
   dependência de ADR restante.
6. ✅ Aplicação UC06 concluída (mais simples, sem novas dependências de
   domínio; `Confirmar`/`Cancelar` já retornam `Resultado`, sem exceção a
   capturar).
7. ✅ Aplicação UC07 concluída (somente leitura; resolução de fuso e
   projeção diária extraídas para `ProjetorDeAgenda`, reaproveitado pelas
   consultas diária e semanal).
8. ✅ Persistência EF Core/PostgreSQL concluída: `ChronosAgendaDbContext`,
   mapeamentos em `Data/Configuracoes` (convenção `snake_case` via
   `EFCore.NamingConventions`), migration inicial em `Data/EF/Migrations`,
   repositórios concretos, `UnidadeDeTrabalho`, `ProvedorDataHoraUtc`,
   `IServicoAutenticacao`/`ServicoAutenticacao` sobre o ASP.NET Core Identity
   e composição de `IContextoUsuario` por requisição na Api (ADR 0006). A
   garantia transacional contra sobreposição sob concorrência real
   (**[ADR pendente #1]**) segue em aberto — Marco 2, item 4 de `dominio.md`.

## 5. Decisões que bloqueiam partes deste plano

Reproduzido de `dominio.md`, mapeado ao que cada ADR destrava aqui:

| ADR pendente | Bloqueia |
| --- | --- |
| #1 — garantia transacional de não sobreposição | Fechamento definitivo de UC04/UC05 sob concorrência real (RN02); a checagem de conflito hoje é aplicação-only (`BuscarAtivosSobrepostosAsync` seguido de `Adicionar`/`Atualizar`), sem lock/constraint de banco |
| #2 — mecanismo de autenticação/sessão e forma da associação usuário↔organização | **Resolvido pelo ADR 0006**: tokens de portador nativos do ASP.NET Core Identity, `IMembroOrganizacaoRepositorio` implementado sobre `membros_organizacao` e `IContextoUsuario` composto por `ResolucaoContextoUsuarioMiddleware` na Api. Papéis de autorização dentro desse vínculo (ex.: proprietário) seguem em aberto para quando um caso de uso concreto exigir (ver Fase A item 4) |
| #3 — exclusão, retenção e dados pessoais | Política de exclusão de UC06/UC07 além do MVP (Marco 3) |
| #4 — fuso horário e horários ambíguos/inexistentes | **Resolvido para UC04/UC05 pelo ADR 0005** (ver seção UC04 acima): entrada com offset explícito remove a ambiguidade na conversão local→UTC, e a conversão UTC→local (RN07) nunca foi ambígua. Os demais itens do ADR pendente #4 em `dominio.md` — bloqueios, feriados e exceções de disponibilidade — seguem em aberto, fora do escopo do MVP |
| #5 — duração manual e snapshots comerciais | Campo opcional `DuracaoManualJustificativa`, ainda não adicionado ao snapshot da Fase B item 7 |

Nenhum desses ADRs bloqueia UC02, UC03, UC04, UC06 ou UC07 — esses casos de
uso podem avançar imediatamente após a Fase A/B (concluídas) e a Fase 0 de
aplicação.
