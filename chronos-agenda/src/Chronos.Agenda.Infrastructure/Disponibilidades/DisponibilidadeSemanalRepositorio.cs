using Chronos.Agenda.Application.Disponibilidades.Contratos;
using Chronos.Agenda.Domain.Disponibilidades.Entidades;
using Chronos.Agenda.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Agenda.Infrastructure.Disponibilidades;

/// <summary>Implementação de <see cref="IDisponibilidadeSemanalRepositorio"/> sobre o EF Core/PostgreSQL. Toda
/// busca filtra explicitamente pela organização informada (RN01, ADR 0001).</summary>
public sealed class DisponibilidadeSemanalRepositorio(ChronosAgendaDbContext dbContext) : IDisponibilidadeSemanalRepositorio
{
    public async Task AdicionarAsync(DisponibilidadeSemanal disponibilidade, CancellationToken cancellationToken)
    {
        await dbContext.DisponibilidadesSemanais.AddAsync(disponibilidade, cancellationToken);
    }

    public Task AtualizarAsync(DisponibilidadeSemanal disponibilidade, CancellationToken cancellationToken)
    {
        if (dbContext.Entry(disponibilidade).State == EntityState.Detached)
        {
            dbContext.DisponibilidadesSemanais.Update(disponibilidade);
        }

        return Task.CompletedTask;
    }

    public Task RemoverAsync(DisponibilidadeSemanal disponibilidade, CancellationToken cancellationToken)
    {
        dbContext.DisponibilidadesSemanais.Remove(disponibilidade);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyCollection<DisponibilidadeSemanal>> BuscarPorProfissionalEDiaAsync(
        Guid organizacaoId, Guid profissionalId, DayOfWeek diaDaSemana, CancellationToken cancellationToken)
    {
        return await dbContext.DisponibilidadesSemanais
            .Where(d => d.OrganizacaoId == organizacaoId && d.ProfissionalId == profissionalId && d.DiaDaSemana == diaDaSemana)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<DisponibilidadeSemanal>> BuscarPorProfissionalAsync(
        Guid organizacaoId, Guid profissionalId, CancellationToken cancellationToken)
    {
        return await dbContext.DisponibilidadesSemanais
            .Where(d => d.OrganizacaoId == organizacaoId && d.ProfissionalId == profissionalId)
            .ToListAsync(cancellationToken);
    }
}
