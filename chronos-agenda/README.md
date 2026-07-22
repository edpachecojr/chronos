# Chronos Agenda

Solução .NET 10 do backend de agendamentos do Chronos.

## Estrutura

- `src/Chronos.Agenda.Domain`: regras e modelo de negócio sem dependências externas;
- `src/Chronos.Agenda.Application`: fronteira dos casos de uso; contém os
  fundamentos transversais (portas de repositório, unidade de trabalho e
  resolução do tenant corrente) e o primeiro caso de uso implementado, o
  onboarding de organização (UC01);
- `src/Chronos.Agenda.Infrastructure`: integrações técnicas, ainda sem implementação;
- `src/Chronos.Agenda.Api`: host de endpoints, ainda sem endpoints;
- `tests/Chronos.Agenda.Domain.Tests`: testes unitários do domínio;
- `tests/Chronos.Agenda.Application.Tests`: testes unitários da aplicação.

## Modelo inicial

O domínio contém organizações, profissionais, disponibilidades semanais,
serviços e agendamentos. O período do agendamento é UTC, com início e fim
explícitos e duração calculada; ele preserva o preço praticado, a pessoa
atendida e o local conforme a modalidade do serviço no momento da reserva.
`Agendamento.ConflitaCom` compara intervalos ativos para o mesmo profissional;
a garantia transacional contra corridas de concorrência continua pendente de ADR
específico.

A exclusão de agendamentos ainda não foi modelada como exclusão física ou
lógica, pois a política de retenção é uma decisão futura do ADR 0001.

## Testes

```sh
dotnet test Chronos.Agenda.slnx
```
