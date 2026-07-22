# ADR 0005: Formato de data/hora na API e conversão de fuso horário

- Status: Aceito
- Data: 2026-07-22

## Contexto

`docs/backlog/dominio.md` lista como **ADR pendente #4** a "modelagem de fuso
horário e regras para bloqueios, feriados e exceções de disponibilidade".
`docs/backlog/plano-implementacao-mvp.md` amarra parte desse ADR ao passo 2 de
UC04/UC05: converter o horário informado pelo usuário (no fuso da
organização) para UTC antes de montar `PeriodoAgendamento`, "tratando horário
ambíguo/inexistente de forma explícita" — cenário clássico de mudanças de
horário de verão, em que um horário de parede local corresponde a dois
instantes (ambíguo, na volta do horário de verão) ou a nenhum instante
(inexistente, no avanço do horário de verão).

Esse problema só existe quando a entrada é um horário de parede **sem**
offset (um `DateTime` "ingênuo" que precisa ser interpretado à luz das regras
de DST de um fuso horário). Definindo o contrato de entrada da API para
carregar o offset explícito no próprio valor (ISO 8601 com offset, formato
`AAAA-MM-DDTHH:MM:SS.sss±hh:mm`, mapeado para `DateTimeOffset` em C#), quem
resolve a ambiguidade é o cliente que produziu o offset, não o servidor: um
`DateTimeOffset` já identifica um instante único e determinístico,
independente de qualquer tabela de DST. Convertê-lo para UTC
(`DateTimeOffset.UtcDateTime`) é uma operação total, que nunca falha nem
exige escolha de offset.

Falta ainda decidir o sentido inverso: como devolver, em respostas da API,
um instante armazenado em UTC (sem offset) no fuso horário da organização.
Essa conversão (UTC → local) já foi reconhecida como não-ambígua pela Fase B
item 9 do plano de implementação (usada por `VerificadorDisponibilidade` para
checar a RN07), mas o método de conversão citado lá
(`FusoHorario.ConverterParaLocal`) ainda não existe no código.

## Decisão

- **Entrada (API → Application):** toda data/hora que representa a intenção
  de horário de um usuário (ex.: início de um agendamento) é recebida pela
  API como ISO 8601 com offset explícito (`DateTimeOffset`), nunca como um
  horário de parede "ingênuo". Os comandos de Application (`CriarAgendamentoComando`,
  `ReagendarAgendamentoComando`) recebem esse valor como `DateTimeOffset` e
  convertem para UTC com `.UtcDateTime` antes de montar `PeriodoAgendamento`.
  Não há, portanto, cenário de horário ambíguo/inexistente a tratar nesta
  direção: o cliente já resolveu a ambiguidade ao anexar o offset:
  não é responsabilidade do Chronos validar se aquele offset é o que a regra
  de DST do fuso da organização produziria naquele instante — apenas usá-lo
  para obter o instante UTC correspondente.
- **Saída (Application/Domain → API):** para projetar um instante UTC
  armazenado no fuso horário da organização (ex.: RN07 e, no futuro, UC07),
  adiciona-se `FusoHorario.ConverterParaLocal(DateTime instanteUtc)` em
  `Chronos.Agenda.Domain.Organizacoes.ObjetosValor.FusoHorario`, retornando um
  `DateTimeOffset` com o horário de parede e o offset corretos para aquele
  instante específico (via `TimeZoneInfo.ConvertTimeFromUtc` e
  `TimeZoneInfo.GetUtcOffset`, usando o identificador IANA já validado pelo
  próprio `FusoHorario`). Essa direção nunca é ambígua nem inexistente — todo
  instante UTC tem exatamente um horário de parede e um offset em qualquer
  fuso horário — por isso o método não retorna `Resultado` nem lança exceção
  de domínio esperada.
- A verificação de disponibilidade (RN07, UC04 passo 5) usa
  `FusoHorario.ConverterParaLocal` para obter o início e o fim locais do
  período, deriva o dia da semana do início local e monta a `JanelaHorario`
  correspondente. Se início e fim locais caem em dias diferentes, nenhuma
  `JanelaHorario` pode representar esse intervalo (ela não atravessa meia-
  noite); a Aplicação rejeita esse caso diretamente com
  `Agendamentos.Erros.AgendamentoErros.PeriodoAtravessaMeiaNoite`, sem chamar
  `VerificadorDisponibilidade`.

## Consequências

- `CriarAgendamentoComando`/`ReagendarAgendamentoComando` usam `DateTimeOffset`
  para o início informado, não mais um par "horário local + tratamento de
  ambiguidade". Isso remove o passo de Result para horário ambíguo/inexistente
  do UC04/UC05: a conversão de entrada não pode falhar.
- `FusoHorario` ganha `ConverterParaLocal`, testável isoladamente com
  `TimeZoneInfo` (ex.: instantes em torno de transições de DST de
  `America/Sao_Paulo`), sem depender de infraestrutura.
- A Api (ainda não implementada) deve serializar/desserializar datas como ISO
  8601 com offset (comportamento padrão do `System.Text.Json` para
  `DateTimeOffset`) e usar `FusoHorario.ConverterParaLocal` ao projetar
  instantes armazenados para exibição.
- Este ADR resolve, para o escopo de UC04/UC05, a parte do **ADR pendente #4**
  relativa a horários ambíguos/inexistentes. Os demais itens desse ADR
  pendente em `dominio.md` — bloqueios, feriados e exceções de
  disponibilidade — permanecem em aberto e não são tratados aqui.
- `docs/backlog/plano-implementacao-mvp.md` é atualizado para refletir que o
  passo 2 de UC04/UC05 não depende mais de ADR e para descrever o novo
  formato de comando.
