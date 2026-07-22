# ADR 0002: Organização do domínio por intenção

- Status: Aceito
- Data: 2026-07-21

## Contexto

As features do domínio cresceram além de uma entidade por diretório. Manter
entidades, objetos de valor, eventos e exceções lado a lado dificulta localizar
o papel de cada tipo. Os nomes dos eventos também repetiam a informação já
expressa pela pasta e pelo namespace `EventosDominio`.

## Decisão

Cada feature do domínio será organizada em subpastas por intenção, usando
`Entidades`, `ObjetosValor`, `Enumeracoes`, `EventosDominio` e `Excecoes` quando
aplicáveis. O módulo `Compartilhado` usa a mesma organização para suas
intenções transversais: `Entidades`, `Contratos`, `EventosDominio` e `Excecoes`.
Os namespaces acompanham os diretórios.

Os tipos de evento não terão o sufixo `EventoDominio`, pois o namespace já
elimina a ambiguidade. O contrato comum permanece `IEventoDominio` para deixar
explícito o seu papel fora desse contexto de pasta.

`DomainException` será uma classe base abstrata. Cada regra excepcional terá
uma exceção concreta da feature ou, para regras transversais, da intenção
compartilhada responsável por ela.

## Consequências

- A navegação pelo domínio passa a revelar a intenção de cada tipo.
- Consumidores usam namespaces mais específicos e eventos com nomes mais
  concisos, como `ProfissionalCriado`.
- A captura de `DomainException` continua possível nas fronteiras, enquanto o
  domínio e seus testes comunicam regras concretas por tipos específicos.
