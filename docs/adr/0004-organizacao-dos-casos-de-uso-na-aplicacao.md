# ADR 0004: Organização dos casos de uso na camada de Aplicação

- Status: Aceito
- Data: 2026-07-22

## Contexto

Ao implementar o UC01 (onboard de organização), os artefatos do caso de uso
(comando, resultado e a classe que os orquestra) foram agrupados sob uma
pasta `CasosDeUso` dentro da feature (`Organizacoes/CasosDeUso`), com a classe
de orquestração nomeada `CriarOrganizacaoCasoDeUso`.

Revisando essa decisão, dois problemas apareceram:

- `CasosDeUso` como pasta única dentro da feature agrupa todos os casos de
  uso da feature (`CriarOrganizacao`, `RenomearOrganizacao`, etc.) em um só
  lugar, quando cada caso de uso já é, por si, um agrupamento coeso de
  artefatos (comando, resultado, validador) que só faz sentido junto. a
  pasta extra não comunica nada que o nome do caso de uso já não comunique, e
  tende a virar uma pasta única cada vez maior conforme a feature cresce.
- `CriarOrganizacaoCasoDeUso` nomeia a classe pelo papel genérico
  (`CasoDeUso`) em vez do papel que ela de fato desempenha: orquestrar a
  execução de um comando. `Handler` é o nome que o projeto pretende adotar
  daqui para frente para esse papel, alinhado ao vocabulário mais comum em
  arquiteturas orientadas a comandos/handlers.

## Decisão

- Dentro de cada pasta de feature em `Application`, cada caso de uso ganha
  sua própria pasta nomeada pelo caso de uso (ex.:
  `Organizacoes/CriarOrganizacao`), contendo o comando, o resultado, o
  handler e demais artefatos que só façam sentido para aquele caso de uso
  (ex.: validadores). Não existe mais uma pasta `CasosDeUso` intermediária.
- A classe que orquestra a execução de um caso de uso é sufixada com
  `Handler` (ex.: `CriarOrganizacaoHandler`), não `CasoDeUso`. O método de
  execução permanece `ExecutarAsync`.
- Testes espelham a mesma estrutura de pastas e o mesmo nome de classe (ex.:
  `Organizacoes/CriarOrganizacao/CriarOrganizacaoHandlerTests.cs`), como já
  ocorre com as demais convenções de teste do projeto.

## Consequências

- Cada caso de uso é autocontido em sua própria pasta, tornando explícito
  quais artefatos pertencem a ele sem depender de uma pasta compartilhada
  entre todos os casos de uso da feature.
- O nome `Handler` passa a ser o padrão para toda classe orquestradora de
  caso de uso na Aplicação; novos casos de uso devem seguir essa convenção
  desde a criação, sem revisão futura.
- UC01 (`CriarOrganizacaoHandler`) foi renomeado e reorganizado para refletir
  esta decisão; `docs/backlog/plano-implementacao-mvp.md` foi atualizado para
  citar o novo namespace e nome de classe.
