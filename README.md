# Chronos

Chronos é uma ferramenta de agendamento de serviços presenciais, online ou
domiciliares. O produto é voltado a profissionais individuais e pequenos
negócios com poucos funcionários, como barbearias, estúdios de tatuagem,
manicures, fisioterapeutas, professores particulares, profissionais de dança e
outros prestadores de serviço.

## Objetivo do produto mínimo viável

O MVP permitirá que um profissional:

- crie sua conta e seu Negócio;
- defina dias da semana e janelas de atendimento;
- cadastre e precifique os serviços oferecidos;
- registre e gerencie agendamentos;
- crie, edite, confirme, cancele e exclua um agendamento;
- mantenha a agenda sem sobreposição de horários para o mesmo profissional.

A modelagem deve considerar que um serviço pode ser prestado presencialmente,
online ou em domicílio, sem antecipar complexidade que ainda não seja necessária
ao MVP.

## Fora do escopo atual

As capacidades abaixo podem ser mapeadas para evolução futura, mas não fazem
parte da primeira entrega:

- gestão financeira e de comissões;
- pagamentos online;
- pacotes de serviços;
- alertas e notificações com processamento em segundo plano;
- outras automações ou integrações de maior complexidade.

## Stack definida

### Backend

- .NET 10;
- ASP.NET Core Identity para autenticação, sem o prefixo `AspNet` nas tabelas;
- Entity Framework Core como ORM;
- PostgreSQL como banco de dados;
- endpoints em vez de controllers.

### Frontend

- React 19;
- Tailwind CSS;
- shadcn/ui;
- Lucide React.

O backend inicial está em [`chronos-agenda`](chronos-agenda), como uma solução
.NET 10, e o frontend inicial está em [`chronos-app`](chronos-app), um projeto
Vite + React. As decisões técnicas fundamentais estão registradas em
[`docs/adr/0001-decisoes-arquiteturais-iniciais.md`](docs/adr/0001-decisoes-arquiteturais-iniciais.md)
e, para o sistema de tokens de design do frontend, em
[`docs/adr/0007-frontend-chronos-app-e-tokens-de-design.md`](docs/adr/0007-frontend-chronos-app-e-tokens-de-design.md).

## Executando localmente

Pré-requisitos: [.NET 10 SDK](https://dotnet.microsoft.com/download), [Node.js
20+](https://nodejs.org) com [pnpm](https://pnpm.io), e um PostgreSQL
acessível (local ou em contêiner).

### 1. Banco de dados

Suba um PostgreSQL, por exemplo via Docker:

```sh
docker run -d --name chronos-postgres \
  -e POSTGRES_PASSWORD=<senha-local> \
  -e POSTGRES_DB=chronos_agenda \
  -p 5432:5432 postgres:17-alpine
```

### 2. Backend (`chronos-agenda`)

A string de conexão vem de variável de ambiente, nunca de arquivo versionado
(ver [Segurança e repositório público](#segurança-e-repositório-público)):

```sh
cd chronos-agenda
export ConnectionStrings__ChronosAgenda="Host=localhost;Port=5432;Database=chronos_agenda;Username=postgres;Password=<senha-local>"
dotnet tool install --global dotnet-ef   # uma única vez, se ainda não tiver
dotnet ef database update --project src/Chronos.Agenda.Infrastructure --startup-project src/Chronos.Agenda.Api
dotnet run --project src/Chronos.Agenda.Api
```

A Api sobe em `http://localhost:5080` (porta fixada em
`src/Chronos.Agenda.Api/Properties/launchSettings.json`), já com CORS
liberado para `http://localhost:5173` em ambiente de desenvolvimento
(`src/Chronos.Agenda.Api/appsettings.Development.json`).

### 3. Frontend (`chronos-app`)

```sh
cd chronos-app
pnpm install
cp .env.example .env.development   # ajuste VITE_API_BASE_URL se a Api não estiver em localhost:5080
pnpm dev
```

Acesse `http://localhost:5173`. Na primeira visita, crie uma conta em
`/registro`: o cadastro autentica automaticamente e encaminha para o
onboarding (nome do negócio e do profissional inicial) antes de liberar o
acesso ao produto. Em acessos seguintes, `/login` reconhece a sessão salva e
encaminha direto para a organização já criada.

## Diretrizes de engenharia

O [`CLAUDE.md`](CLAUDE.md) é a fonte canônica das regras de engenharia e deve
orientar toda contribuição. A estrutura de diretórios será definida em conjunto
quando os projetos forem criados, respeitando as convenções dos frameworks e a
separação clara de responsabilidades.

## Segurança e repositório público

Este projeto será público no GitHub. Segredos, tokens, chaves, senhas e dados
sensíveis nunca devem ser versionados nem escritos diretamente no código ou na
documentação. Configurações sensíveis devem vir de mecanismos externos ao
repositório, como variáveis de ambiente ou um gerenciador de segredos, e arquivos
locais que possam contê-las devem ser ignorados pelo Git.

## Decisões arquiteturais

Os Architecture Decision Records (ADRs) ficam em [`docs/adr`](docs/adr). Cada
decisão relevante deve registrar contexto, escolha e consequências para que a
arquitetura evolua de forma explícita e rastreável.
