# ADR 0006: Persistência com EF Core/PostgreSQL e mecanismo de autenticação

- Status: Aceito
- Data: 2026-07-22

## Contexto

`docs/backlog/plano-implementacao-mvp.md` lista, como último item do
sequenciamento recomendado, a implementação da persistência EF Core/PostgreSQL
para a camada de Infraestrutura. `docs/backlog/dominio.md` também registra o
**ADR pendente #2** ("mecanismo de autenticação/sessão"), que bloqueava a
implementação concreta de `IMembroOrganizacaoRepositorio` e a composição de
`IContextoUsuario` por requisição — a única parte de Fase 0 da Aplicação que
dependia de uma decisão de Infraestrutura/Api ainda não tomada.

O ADR 0001 já havia decidido usar ASP.NET Core Identity para autenticação, mas
não o mecanismo de sessão (cookie, JWT customizado ou token de portador nativo
do Identity). Sem essa decisão, não era possível implementar o serviço de
autenticação concreto nem compor `IContextoUsuario` na Api.

## Decisão

### Persistência

- O acesso a dados usa Entity Framework Core sobre PostgreSQL (Npgsql), com a
  convenção `snake_case` aplicada via `EFCore.NamingConventions`
  (`UseSnakeCaseNamingConvention`), cobrindo tabelas, colunas e chaves
  estrangeiras que não recebem nome explícito.
- Todo mapeamento e configuração do EF Core fica em
  `Chronos.Agenda.Infrastructure/Data`, com uma classe
  `IEntityTypeConfiguration<T>` por tipo em `Data/Configuracoes` (e
  `Data/Configuracoes/Identity` para as tabelas do Identity). As migrations
  ficam em `Data/EF/Migrations`.
- As entidades do domínio (`Organizacao`, `Profissional`, `Servico`,
  `DisponibilidadeSemanal`, `Agendamento`) e alguns objetos de valor que
  referenciam outro objeto de valor em seu construtor (`PessoaAtendida`,
  `LocalAtendimento`) ganharam um construtor privado sem parâmetros, usado
  exclusivamente pelo EF Core para materialização. O EF Core não vincula
  objetos de valor (owned types) a parâmetros de construtor da entidade
  proprietária — apenas propriedades escalares —, então a alternativa ao
  construtor vazio seria abrir mão de objetos de valor imutáveis ou duplicar
  o modelo de domínio em um modelo de persistência separado. O construtor
  fica `private`, nunca é usado pelo domínio ou pela aplicação, e a classe
  base `Entidade` ganhou o mesmo recurso para as propriedades comuns (`Id`,
  `Auditoria`).
- As tabelas do ASP.NET Core Identity são renomeadas sem o prefixo `AspNet`
  (ADR 0001): `usuarios`, `roles`, `user_roles`, `user_claims`,
  `user_logins`, `user_tokens`, `role_claims`. As chaves estrangeiras dessas
  tabelas para `usuarios` também são renomeadas explicitamente
  (`fk_user_claims_usuarios_user_id` etc.), pois a convenção padrão do EF
  Core nomeia a chave estrangeira antes da tabela ser renomeada.
- Campos de texto expostos antes da autenticação (nome, e-mail, hash de
  senha, tokens) recebem limite explícito de tamanho (`HasMaxLength`) nas
  classes de configuração, tanto para as entidades do domínio quanto para as
  tabelas do Identity.
- O isolamento por tenant (RN01, ADR 0001) não é implementado como query
  filter global: cada repositório recebe e aplica `OrganizacaoId`
  explicitamente na própria consulta, mantendo a decisão já tomada no ADR
  0001.

### Mecanismo de autenticação/sessão (resolve o ADR pendente #2)

- O ASP.NET Core Identity usa tokens de portador (bearer tokens) nativos do
  Identity como mecanismo de sessão (`AddIdentityApiEndpoints<UsuarioIdentity>`,
  que registra `IdentityConstants.BearerScheme` como esquema padrão de
  autenticação). Não há infraestrutura de JWT customizada: o token é opaco,
  emitido e validado inteiramente pelo Identity.
- Essa escolha evita gerenciar uma chave de assinatura própria como segredo
  adicional e se encaixa no frontend React (ADR 0001), que consome a Api como
  cliente separado, sem depender de cookies same-site.
- A associação usuário↔organização (ADR 0003) é resolvida por
  `IMembroOrganizacaoRepositorio`, implementado sobre uma tabela
  `membros_organizacao` com índice único em `usuario_id` (um usuário pertence
  a, no máximo, uma organização neste escopo do MVP).
- `IContextoUsuario` é composto uma única vez por requisição autenticada, em
  `ResolucaoContextoUsuarioMiddleware` (Api), que extrai o identificador do
  usuário da claim `NameIdentifier` e consulta
  `IMembroOrganizacaoRepositorio.BuscarOrganizacaoIdDoUsuarioAsync`,
  registrando o resultado em `ContextoUsuarioAcessor` (um retentor `scoped`).
  Isso resolve a tensão, já prevista na Fase 0 do plano de implementação,
  entre `IContextoUsuario.ObterOrganizacaoId()` ser síncrono no domínio e a
  consulta ao vínculo ser assíncrona.
- Um novo contrato de Aplicação, `Compartilhado.Contratos.IServicoAutenticacao`
  (`CriarUsuarioAsync`, `AutenticarAsync`, `BuscarPorEmailAsync`), expõe as
  operações de autenticação que os casos de uso de onboarding/login vão
  precisar. A implementação concreta (`Infrastructure.Identity.ServicoAutenticacao`)
  usa `UserManager<UsuarioIdentity>`/`SignInManager<UsuarioIdentity>`. Falhas
  de credenciais inválidas retornam sempre a mesma mensagem genérica,
  independentemente de o e-mail existir ou a senha estar errada, para não
  expor se um e-mail está cadastrado.
- Os endpoints da Api (`MapIdentityApi<UsuarioIdentity>` e os demais) não
  fazem parte deste ADR nem desta etapa: apenas a injeção de dependências e a
  configuração de autenticação/autorização foram implementadas.

## Consequências

- `docs/backlog/plano-implementacao-mvp.md` é atualizado para marcar a
  persistência EF Core/PostgreSQL como concluída e remover o ADR pendente #2
  da lista de bloqueios (a parte de mecanismo de sessão; a modelagem de
  papéis de autorização dentro de `membros_organizacao`, se necessária,
  permanece em aberto para quando um caso de uso concreto exigir).
- Entidades de domínio carregam um construtor privado adicional cujo único
  consumidor é o EF Core via reflection; isso é um acoplamento mínimo e
  unidirecional ao mecanismo de materialização do ORM (a entidade não
  referencia nenhum tipo do EF Core), não ao comportamento de domínio.
- Testes de domínio e aplicação existentes não foram afetados: os
  construtores completos usados pelas fábricas (`Organizacao.Criar`, etc.)
  permanecem inalterados.
- A troca futura do mecanismo de sessão (ex.: cookies para um cenário
  diferente do atual) exigiria revisar apenas `AdicionarIdentity` (Infraestrutura)
  e o pipeline de autenticação da Api, sem alterar `IServicoAutenticacao` nem
  os casos de uso que dependem de `IContextoUsuario`.
