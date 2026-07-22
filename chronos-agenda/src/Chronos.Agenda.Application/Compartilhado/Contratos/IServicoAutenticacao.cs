using Chronos.Agenda.Domain.Compartilhado.Resultados;

namespace Chronos.Agenda.Application.Compartilhado.Contratos;

/// <summary>Operações de autenticação necessárias para os casos de uso do MVP: criar, recuperar e autenticar um
/// usuário. A concessão de acesso por organização (autorização multi-tenant, RN01) é resolvida à parte por
/// <see cref="Domain.Compartilhado.Contratos.IContextoUsuario"/> e <see cref="IMembroOrganizacaoRepositorio"/>, não
/// por este contrato. A implementação concreta pertence à Infraestrutura (ASP.NET Core Identity, ADR 0001).</summary>
public interface IServicoAutenticacao
{
    /// <summary>Cria um novo usuário autenticável com o e-mail e a senha informados.</summary>
    /// <example><code>var resultado = await servicoAutenticacao.CriarUsuarioAsync(email, senha, cancellationToken);</code></example>
    Task<Resultado<Guid>> CriarUsuarioAsync(string email, string senha, CancellationToken cancellationToken);

    /// <summary>Autentica um usuário existente pelo e-mail e senha informados.</summary>
    /// <example><code>var resultado = await servicoAutenticacao.AutenticarAsync(email, senha, cancellationToken);</code></example>
    Task<Resultado<Guid>> AutenticarAsync(string email, string senha, CancellationToken cancellationToken);

    /// <summary>Recupera o identificador de um usuário existente pelo e-mail, ou nulo se não houver cadastro.</summary>
    /// <example><code>var usuarioId = await servicoAutenticacao.BuscarPorEmailAsync(email, cancellationToken);</code></example>
    Task<Guid?> BuscarPorEmailAsync(string email, CancellationToken cancellationToken);
}
