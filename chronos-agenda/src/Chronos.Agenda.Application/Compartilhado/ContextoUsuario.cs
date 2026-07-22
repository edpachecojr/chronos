using Chronos.Agenda.Domain.Compartilhado.Contratos;

namespace Chronos.Agenda.Application.Compartilhado;

/// <summary>
/// Implementação de <see cref="IContextoUsuario"/> que apenas devolve valores
/// já resolvidos para a requisição corrente. A resolução em si — extrair o
/// usuário autenticado do contexto HTTP e consultar
/// <see cref="Contratos.IMembroOrganizacaoRepositorio"/> — é assíncrona e deve
/// acontecer uma única vez por requisição, fora desta classe (na composição
/// da Api), porque <see cref="IContextoUsuario.ObterOrganizacaoId"/> é
/// síncrono no contrato de domínio.
/// </summary>
/// <example><code>var contexto = new ContextoUsuario(usuarioId, organizacaoId);</code></example>
public sealed class ContextoUsuario(Guid usuarioId, Guid organizacaoId) : IContextoUsuario
{
    public Guid UsuarioId { get; } = usuarioId;

    public Guid ObterOrganizacaoId() => organizacaoId;
}
