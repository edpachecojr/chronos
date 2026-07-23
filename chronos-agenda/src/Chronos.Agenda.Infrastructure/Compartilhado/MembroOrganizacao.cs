using Chronos.Agenda.Application.Compartilhado;

namespace Chronos.Agenda.Infrastructure.Compartilhado;

/// <summary>Persiste o vínculo entre um usuário autenticado (Identity) e a organização à qual pertence, com o
/// papel de autorização que ele assume nela (ADR 0003). Não é uma entidade de domínio — apenas o registro que
/// sustenta <see cref="Application.Compartilhado.Contratos.IMembroOrganizacaoRepositorio"/>.</summary>
public sealed class MembroOrganizacao
{
    public MembroOrganizacao(Guid id, Guid usuarioId, Guid organizacaoId, PapelMembroOrganizacao papel, DateTime criadoEmUtc)
    {
        Id = id;
        UsuarioId = usuarioId;
        OrganizacaoId = organizacaoId;
        Papel = papel;
        CriadoEmUtc = criadoEmUtc;
    }

    public Guid Id { get; private set; }
    public Guid UsuarioId { get; private set; }
    public Guid OrganizacaoId { get; private set; }
    public PapelMembroOrganizacao Papel { get; private set; }
    public DateTime CriadoEmUtc { get; private set; }
}
