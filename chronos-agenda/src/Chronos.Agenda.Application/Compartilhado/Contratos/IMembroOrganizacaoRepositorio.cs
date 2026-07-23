namespace Chronos.Agenda.Application.Compartilhado.Contratos;

/// <summary>
/// Acesso ao vínculo persistido entre um usuário autenticado (Identity) e a
/// organização à qual pertence, usado para resolver o tenant corrente (RN01,
/// ADR 0003) e o papel de autorização (proprietário vs. membro) dentro desse
/// vínculo. Não é um repositório de agregado de domínio — o vínculo não é
/// modelado no domínio — apenas a leitura e o registro que a aplicação
/// precisa.
/// </summary>
public interface IMembroOrganizacaoRepositorio
{
    /// <summary>Registra o vínculo entre um usuário e a organização à qual passa a pertencer, com o papel de
    /// autorização que ele assume nela.</summary>
    /// <example><code>await membros.AdicionarAsync(usuarioId, organizacaoId, PapelMembroOrganizacao.Proprietario, cancellationToken);</code></example>
    Task AdicionarAsync(Guid usuarioId, Guid organizacaoId, PapelMembroOrganizacao papel, CancellationToken cancellationToken);

    /// <summary>Resolve a organização vinculada a um usuário, ou nulo se não houver vínculo.</summary>
    /// <example><code>var organizacaoId = await membros.BuscarOrganizacaoIdDoUsuarioAsync(usuarioId, cancellationToken);</code></example>
    Task<Guid?> BuscarOrganizacaoIdDoUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken);

    /// <summary>Resolve o papel de autorização de um usuário no vínculo, ou nulo se não houver vínculo.</summary>
    /// <example><code>var papel = await membros.BuscarPapelDoUsuarioAsync(usuarioId, cancellationToken);</code></example>
    Task<PapelMembroOrganizacao?> BuscarPapelDoUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken);
}
