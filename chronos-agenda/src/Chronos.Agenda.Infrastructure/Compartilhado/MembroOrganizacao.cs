namespace Chronos.Agenda.Infrastructure.Compartilhado;

/// <summary>Persiste o vínculo entre um usuário autenticado (Identity) e a organização à qual pertence (ADR 0003).
/// Não é uma entidade de domínio — apenas o registro que sustenta <see cref="Application.Compartilhado.Contratos.IMembroOrganizacaoRepositorio"/>.</summary>
public sealed class MembroOrganizacao
{
    public MembroOrganizacao(Guid id, Guid usuarioId, Guid organizacaoId, DateTime criadoEmUtc)
    {
        Id = id;
        UsuarioId = usuarioId;
        OrganizacaoId = organizacaoId;
        CriadoEmUtc = criadoEmUtc;
    }

    public Guid Id { get; private set; }
    public Guid UsuarioId { get; private set; }
    public Guid OrganizacaoId { get; private set; }
    public DateTime CriadoEmUtc { get; private set; }
}
