namespace Chronos.Agenda.Application.Compartilhado.Contratos;

/// <summary>
/// Acesso ao vínculo persistido entre um usuário autenticado (Identity) e a
/// organização à qual pertence, usado para resolver o tenant corrente (RN01,
/// ADR 0003). Não é um repositório de agregado de domínio — o vínculo não é
/// modelado no domínio — apenas a leitura e o registro que a aplicação
/// precisa. Papéis de autorização (ex.: proprietário) ainda não fazem parte
/// deste contrato; entram quando um caso de uso de autorização exigir.
/// </summary>
public interface IMembroOrganizacaoRepositorio
{
    /// <summary>Registra o vínculo entre um usuário e a organização à qual passa a pertencer.</summary>
    /// <example><code>await membros.AdicionarAsync(usuarioId, organizacaoId, cancellationToken);</code></example>
    Task AdicionarAsync(Guid usuarioId, Guid organizacaoId, CancellationToken cancellationToken);

    /// <summary>Resolve a organização vinculada a um usuário, ou nulo se não houver vínculo.</summary>
    /// <example><code>var organizacaoId = await membros.BuscarOrganizacaoIdDoUsuarioAsync(usuarioId, cancellationToken);</code></example>
    Task<Guid?> BuscarOrganizacaoIdDoUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken);
}
