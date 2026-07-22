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
.NET 10. As decisões técnicas fundamentais estão registradas em
[`docs/adr/0001-decisoes-arquiteturais-iniciais.md`](docs/adr/0001-decisoes-arquiteturais-iniciais.md).

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
