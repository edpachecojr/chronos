using Chronos.Agenda.Application.Servicos.Contratos;
using Chronos.Agenda.Domain.Servicos.Entidades;
using Chronos.Agenda.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Agenda.Infrastructure.Servicos;

/// <summary>Implementação de <see cref="IServicoRepositorio"/> sobre o EF Core/PostgreSQL. Toda busca filtra
/// explicitamente pela organização informada (RN01, ADR 0001).</summary>
public sealed class ServicoRepositorio(ChronosAgendaDbContext dbContext) : IServicoRepositorio
{
    public async Task AdicionarAsync(Servico servico, CancellationToken cancellationToken)
    {
        await dbContext.Servicos.AddAsync(servico, cancellationToken);
    }

    public Task AtualizarAsync(Servico servico, CancellationToken cancellationToken)
    {
        if (dbContext.Entry(servico).State == EntityState.Detached)
        {
            dbContext.Servicos.Update(servico);
        }

        return Task.CompletedTask;
    }

    public async Task<Servico?> BuscarPorIdAsync(Guid organizacaoId, Guid servicoId, CancellationToken cancellationToken)
    {
        return await dbContext.Servicos
            .FirstOrDefaultAsync(s => s.Id == servicoId && s.OrganizacaoId == organizacaoId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Servico>> BuscarPorProfissionalAsync(Guid organizacaoId, Guid profissionalId, CancellationToken cancellationToken)
    {
        return await dbContext.Servicos
            .Where(s => s.OrganizacaoId == organizacaoId && s.ProfissionalId == profissionalId)
            .ToListAsync(cancellationToken);
    }
}
