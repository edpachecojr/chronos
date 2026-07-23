using Chronos.Agenda.Application.Profissionais.Contratos;
using Chronos.Agenda.Domain.Profissionais.Entidades;
using Chronos.Agenda.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Agenda.Infrastructure.Profissionais;

/// <summary>Implementação de <see cref="IProfissionalRepositorio"/> sobre o EF Core/PostgreSQL. Toda busca filtra
/// explicitamente pela organização informada (RN01, ADR 0001).</summary>
public sealed class ProfissionalRepositorio(ChronosAgendaDbContext dbContext) : IProfissionalRepositorio
{
    public async Task AdicionarAsync(Profissional profissional, CancellationToken cancellationToken)
    {
        await dbContext.Profissionais.AddAsync(profissional, cancellationToken);
    }

    public async Task<Profissional?> BuscarPorIdAsync(Guid organizacaoId, Guid profissionalId, CancellationToken cancellationToken)
    {
        return await dbContext.Profissionais
            .FirstOrDefaultAsync(p => p.Id == profissionalId && p.OrganizacaoId == organizacaoId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Profissional>> BuscarPorOrganizacaoAsync(Guid organizacaoId, CancellationToken cancellationToken)
    {
        return await dbContext.Profissionais
            .Where(p => p.OrganizacaoId == organizacaoId)
            .ToListAsync(cancellationToken);
    }
}
