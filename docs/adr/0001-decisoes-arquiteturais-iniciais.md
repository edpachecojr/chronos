# ADR 0001: Decisões arquiteturais iniciais

- Status: Aceito
- Data: 2026-07-21

## Contexto

Chronos atenderá profissionais individuais e pequenos negócios que precisam
configurar sua disponibilidade, oferecer serviços e controlar agendamentos. O
MVP precisa impedir conflitos de agenda para um mesmo profissional e criar uma
base segura para múltiplas organizações, sem antecipar funcionalidades como
pagamentos, gestão financeira, comissões, pacotes, alertas ou processamento em
segundo plano.

Como o repositório será público, a arquitetura e a configuração também devem
prevenir a exposição de credenciais e outros dados sensíveis.

## Decisão

### Plataforma e persistência

- O backend usará .NET 10.
- A autenticação usará ASP.NET Core Identity.
- As tabelas do Identity terão nomes explícitos sem o prefixo `AspNet`.
- O acesso a dados usará Entity Framework Core.
- O banco de dados será PostgreSQL.
- A convenção relacional será `snake_case`, com nomes de tabelas no plural.
- Entidades serão nomeadas no singular.
- Haverá uma entidade base responsável pela identidade dos objetos e pelos dados
  comuns de auditoria. Os campos exatos serão definidos junto com o modelo de
  domínio, evitando incluir responsabilidades não compartilhadas por todas as
  entidades.

### API e tratamento de falhas

- A API será exposta por endpoints, sem controllers.
- Falhas esperadas serão representadas com Result Pattern.
- Os tipos de erro do Result Pattern serão `record`, permitindo valores
  imutáveis, explícitos e comparáveis.
- Erros inesperados de regras do domínio serão representados por exceções de
  domínio com contexto suficiente para diagnóstico.
- O domínio poderá emitir eventos de domínio para fatos relevantes que precisem
  ser observados por outras partes da aplicação.

### Multi-tenancy

- O sistema será multi-tenant desde o início, tendo a organização como limite de
  propriedade e acesso.
- Entidades pertencentes a uma organização implementarão
  `IPertenceOrganizacao`.
- A camada de aplicação será responsável por validar o pertencimento à
  organização e restringir explicitamente cada leitura e escrita ao tenant
  corrente.
- O isolamento não será delegado ao banco de dados.
- Não serão usados query filters globais nem interceptors para aplicar o tenant
  implicitamente.
- Consultas, comandos e testes devem tornar visível o identificador da
  organização e comprovar que dados de outro tenant não podem ser acessados ou
  alterados.

### Integridade dos agendamentos

- A aplicação deve impedir dois agendamentos conflitantes para o mesmo
  profissional no mesmo intervalo de tempo.
- A regra deve considerar a duração do serviço, e não apenas a igualdade do
  horário inicial.
- A consistência diante de requisições concorrentes deverá ser definida em ADR
  próprio ao modelar os agendamentos e sua estratégia transacional.

### Frontend

- O frontend usará React 19, Tailwind CSS, shadcn/ui e Lucide React.
- Esta decisão define apenas a stack. Nenhum código ou estrutura de frontend faz
  parte deste ADR inicial.

### Simplicidade e segurança

- A implementação deve ser explícita e evitar indireções desnecessárias,
  convenções ocultas e lógica "mágica".
- Segredos e chaves nunca serão mantidos no código ou versionados. Configuração
  sensível será fornecida externamente, por variáveis de ambiente ou gerenciador
  de segredos.

## Consequências

- A filtragem por organização será repetida de forma intencional nas fronteiras
  da aplicação, tornando o isolamento auditável e testável.
- A ausência de query filters e interceptors reduz comportamento implícito, mas
  exige testes consistentes contra vazamento entre tenants.
- Personalizar os nomes das tabelas do Identity exigirá mapeamento explícito no
  EF Core.
- A convenção de pluralização e `snake_case` deverá ser centralizada e validada
  nas migrações.
- Result Pattern evita exceções como controle de fluxo para falhas conhecidas;
  exceções de domínio ficam reservadas aos cenários não tratados.
- Eventos de domínio criam pontos explícitos de extensão, mas só devem ser
  adicionados quando houver um fato relevante e um consumidor real.
- A proteção contra sobreposição precisa de uma decisão posterior sobre
  concorrência e garantias transacionais no PostgreSQL.

## Alternativas consideradas

### Controllers

Rejeitados em favor de endpoints menores e organizados por capacidade. Essa
escolha reduz cerimônia, mas não elimina a necessidade de separar validação,
aplicação e domínio.

### Query filters ou interceptors para multi-tenancy

Rejeitados porque ocultariam uma regra crítica de autorização e pertencimento.
A restrição explícita na aplicação favorece leitura, revisão e testes.

### Isolamento de tenant gerenciado pelo banco

Não adotado. A aplicação será a responsável pelo pertencimento e isolamento dos
dados, mantendo essa decisão visível nos casos de uso.

## Decisões futuras relacionadas

ADRs posteriores deverão detalhar, quando o domínio for modelado:

- estrutura das camadas e convenção de diretórios;
- campos da entidade base e política de auditoria;
- modelagem de organizações, profissionais, disponibilidade e serviços;
- estados e transições do ciclo de vida de um agendamento;
- prevenção transacional de sobreposição em cenários concorrentes;
- autenticação, autorização e resolução explícita do tenant;
- política de exclusão e retenção de dados;
- observabilidade e tratamento de dados pessoais.
