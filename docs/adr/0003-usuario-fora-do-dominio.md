# ADR 0003: Usuário não é um conceito de domínio

- Status: Aceito
- Data: 2026-07-22

## Contexto

Ao modelar o vínculo entre quem acessa o Chronos e a organização à qual
pertence, foi criada uma entidade `Usuario` no domínio (`Usuarios.Entidades`),
implementando `IPertenceOrganizacao` como `Profissional` e `Agendamento`, com
um `PapelUsuario` (`Proprietario`/`Membro`) para distinguir quem administra a
organização dos demais.

Revisando essa decisão, o problema apareceu: "usuário" não é um conceito que
os especialistas de domínio do Chronos reconhecem. Quem participa do negócio
são papéis com comportamento e regras próprias — o profissional que presta o
serviço, o proprietário que administra a organização, a pessoa atendida
(paciente, aluno, cliente) que recebe o serviço. "Usuário" é, em vez disso,
quem se autentica para operar o sistema: um conceito de acesso, não de
negócio. O ADR 0001 já havia reservado esse conceito para o ASP.NET Core
Identity, na camada de Infraestrutura.

A entidade `Usuario` criada misturava as duas coisas: identidade de
autenticação (que pertence ao Identity) e papel de negócio dentro da
organização (que já tem — ou terá — representação própria: `Profissional`
existe; uma futura pessoa atendida persistente evolui de `PessoaAtendida`,
hoje objeto de valor em `Agendamentos`; "proprietário" é, no fundo, uma
questão de autorização — quem pode administrar a organização — não uma
entidade com comportamento de domínio próprio).

Nenhuma regra de domínio hoje depende de "quem é o usuário logado" como
invariante a ser protegida pela própria entidade. As únicas necessidades
identificadas são de autorização (o que este usuário pode fazer) e de
resolução de tenant (a qual organização este usuário pertence), ambas
resolvidas na fronteira da aplicação, não dentro do domínio.

## Decisão

- O domínio não terá uma entidade `Usuario`. A entidade criada
  (`Usuarios.Entidades.Usuario`), o enum `PapelUsuario`, o evento
  `UsuarioCriado` e a exceção `OrganizacaoUsuarioInvalidaException` foram
  removidos.
- Identidade e autenticação continuam exclusivamente no ASP.NET Core
  Identity (Infraestrutura), conforme ADR 0001. O domínio não referencia
  tabelas ou tipos do Identity.
- O vínculo entre um usuário autenticado (Identity) e a organização corrente
  — incluindo quem é proprietário — é resolvido inteiramente em
  Application/Infrastructure, por meio de uma associação simples (por
  exemplo, uma tabela `membros_organizacao` com `usuario_id`,
  `organizacao_id` e um papel), não por um agregado de domínio.
- O domínio mantém o contrato `Compartilhado.Contratos.IContextoUsuario`
  (`Guid UsuarioId`, `Guid ObterOrganizacaoId()`). Ele expõe o identificador
  do usuário autenticado — um dado que a sessão já carrega — e o método para
  resolver a organização corrente, sem o domínio precisar conhecer como esse
  usuário é armazenado ou autenticado. A implementação concreta pertence à
  Application (orquestração do contexto de uso) e à Infraestrutura
  (extração do usuário autenticado e consulta ao vínculo persistido).
- Papéis de negócio continuam a ser modelados individualmente, no domínio,
  apenas quando surgir comportamento ou regra que os justifique:
  `Profissional` já existe; um agregado para a pessoa atendida (evoluindo de
  `PessoaAtendida`) e qualquer necessidade de "proprietário" como conceito
  de domínio (e não só de autorização) serão avaliados quando o caso de uso
  concreto exigir, não antecipadamente.

## Consequências

- O domínio permanece livre de um conceito que não pertence à linguagem
  ubíqua do negócio, evitando um agregado genérico sem comportamento real
  (um "God object" em potencial para futuras regras de autorização).
- A autorização (quem pode fazer o quê) e a multi-tenancy por associação
  usuário↔organização tornam-se responsabilidade explícita de
  Application/Infrastructure, coerente com o ADR 0001 (isolamento de tenant
  não é delegado ao domínio nem ao banco).
- `IContextoUsuario` isola os casos de uso do mecanismo de autenticação; a
  troca de estratégia (ex.: JWT, cookies, múltiplas organizações por usuário)
  não exige mudança no domínio.
- Se, no futuro, surgir uma regra de domínio genuína que dependa do "papel do
  usuário" como invariante (não apenas autorização de rota), a modelagem
  deve nascer no agregado de negócio pertinente (`Profissional`, pessoa
  atendida, ou um novo conceito nomeado pela linguagem do domínio) — nunca
  reintroduzindo um `Usuario` genérico.
- `docs/backlog/plano-implementacao-mvp.md` deve ser atualizado para refletir
  que o vínculo usuário↔organização é responsabilidade de
  Application/Infrastructure, mantendo apenas `IContextoUsuario` como o que
  já existe no domínio.
