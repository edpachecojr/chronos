# Chronos Agenda

Solução .NET 10 do backend de agendamentos do Chronos.

## Estrutura

- `src/Chronos.Agenda.Domain`: regras e modelo de negócio sem dependências externas;
- `src/Chronos.Agenda.Application`: fronteira dos casos de uso (UC01-UC07):
  portas de repositório, unidade de trabalho, resolução do tenant corrente e
  os handlers de cada caso de uso, registrados no contêiner de injeção de
  dependências por `Extensions.AdicionarCasosDeUso`;
- `src/Chronos.Agenda.Infrastructure`: persistência EF Core/PostgreSQL
  (`snake_case`), repositórios concretos, unidade de trabalho e autenticação
  sobre o ASP.NET Core Identity;
- `src/Chronos.Agenda.Api`: host de endpoints minimalistas, sem controllers
  (ADR 0001). Cada endpoint implementa `Endpoints.IEndpoint` com um membro
  estático e é composto, um a um e versionado por prefixo de rota (`v1/...`),
  em `Endpoints.Endpoint.MapearEndpoints`; falhas esperadas (Result Pattern)
  são traduzidas para HTTP por `Erros.ResultadoHttpExtensions`, e exceções não
  tratadas por `ExceptionHandling.TratadorGlobalDeExcecoes`;
- `tests/Chronos.Agenda.Domain.Tests`: testes unitários do domínio;
- `tests/Chronos.Agenda.Application.Tests`: testes unitários da aplicação;
- `tests/Chronos.Agenda.Api.Tests`: testes unitários da Api (tradução de
  `Resultado`/`Erro` para respostas HTTP).

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

## Executando localmente

A Api precisa de um PostgreSQL acessível e da string de conexão em variável
de ambiente (nunca em arquivo versionado):

```sh
export ConnectionStrings__ChronosAgenda="Host=localhost;Port=5432;Database=chronos_agenda;Username=postgres;Password=<senha-local>"
dotnet ef database update --project src/Chronos.Agenda.Infrastructure --startup-project src/Chronos.Agenda.Api
dotnet run --project src/Chronos.Agenda.Api
```

Sobe em `http://localhost:5080` (porta fixada em
`src/Chronos.Agenda.Api/Properties/launchSettings.json`), com CORS liberado
para `http://localhost:5173` em desenvolvimento
(`src/Chronos.Agenda.Api/appsettings.Development.json`). Guia completo,
incluindo o frontend, no [README raiz](../README.md#executando-localmente).

## Testes

```sh
dotnet test Chronos.Agenda.slnx
```
