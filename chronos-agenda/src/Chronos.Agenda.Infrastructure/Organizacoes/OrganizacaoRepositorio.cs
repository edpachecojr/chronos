using Chronos.Agenda.Application.Organizacoes.Contratos;
using Chronos.Agenda.Domain.Organizacoes.Entidades;
using Chronos.Agenda.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Agenda.Infrastructure.Organizacoes;

/// <summary>Implementação de <see cref="IOrganizacaoRepositorio"/> sobre o EF Core/PostgreSQL.</summary>
public sealed class OrganizacaoRepositorio(ChronosAgendaDbContext dbContext) : IOrganizacaoRepositorio
{
    public async Task AdicionarAsync(Organizacao organizacao, CancellationToken cancellationToken)
    {
        await dbContext.Organizacoes.AddAsync(organizacao, cancellationToken);
    }

    public Task AtualizarAsync(Organizacao organizacao, CancellationToken cancellationToken)
    {
        if (dbContext.Entry(organizacao).State == EntityState.Detached)
        {
            dbContext.Organizacoes.Update(organizacao);
        }

        return Task.CompletedTask;
    }

    public async Task<Organizacao?> BuscarPorIdAsync(Guid organizacaoId, CancellationToken cancellationToken)
    {
        return await dbContext.Organizacoes
            .FirstOrDefaultAsync(o => o.Id == organizacaoId, cancellationToken);
    }
}
