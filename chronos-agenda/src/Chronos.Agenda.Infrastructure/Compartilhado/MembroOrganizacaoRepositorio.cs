using Chronos.Agenda.Application.Compartilhado;
using Chronos.Agenda.Application.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Agenda.Infrastructure.Compartilhado;

/// <summary>Persiste o vínculo usuário↔organização em <c>membros_organizacao</c> (ADR 0003), usado para resolver o
/// tenant corrente (RN01) e o papel de autorização (proprietário vs. membro) dentro desse vínculo.</summary>
public sealed class MembroOrganizacaoRepositorio(ChronosAgendaDbContext dbContext, IProvedorDataHora provedorDataHora)
    : IMembroOrganizacaoRepositorio
{
    public async Task AdicionarAsync(Guid usuarioId, Guid organizacaoId, PapelMembroOrganizacao papel, CancellationToken cancellationToken)
    {
        var membro = new MembroOrganizacao(Guid.NewGuid(), usuarioId, organizacaoId, papel, provedorDataHora.UtcNow);
        await dbContext.MembrosOrganizacao.AddAsync(membro, cancellationToken);
    }

    public async Task<Guid?> BuscarOrganizacaoIdDoUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken)
    {
        return await dbContext.MembrosOrganizacao
            .Where(m => m.UsuarioId == usuarioId)
            .Select(m => (Guid?)m.OrganizacaoId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PapelMembroOrganizacao?> BuscarPapelDoUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken)
    {
        return await dbContext.MembrosOrganizacao
            .Where(m => m.UsuarioId == usuarioId)
            .Select(m => (PapelMembroOrganizacao?)m.Papel)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
