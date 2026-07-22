namespace Chronos.Agenda.Application.Organizacoes.CasosDeUso;

/// <summary>
/// Dados para o onboarding de uma organização (UC01): cria a organização e
/// registra, como seu primeiro profissional, quem a está configurando.
/// </summary>
/// <example><code>
/// var comando = new CriarOrganizacaoComando(usuarioId, "Clínica Bem-Estar", "Dra. Ana Souza");
/// </code></example>
public sealed record CriarOrganizacaoComando(Guid UsuarioId, string Nome, string NomeProfissionalInicial);
