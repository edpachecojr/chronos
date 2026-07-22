using Chronos.Agenda.Application.Agendamentos.Contratos;
using Chronos.Agenda.Domain.Agendamentos.Entidades;
using Chronos.Agenda.Domain.Agendamentos.Enums;
using Chronos.Agenda.Domain.Agendamentos.ObjetosValor;
using Chronos.Agenda.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Agenda.Infrastructure.Agendamentos;

/// <summary>Implementação de <see cref="IAgendamentoRepositorio"/> sobre o EF Core/PostgreSQL. Toda busca filtra
/// explicitamente pela organização informada (RN01, ADR 0001).</summary>
public sealed class AgendamentoRepositorio(ChronosAgendaDbContext dbContext) : IAgendamentoRepositorio
{
    public async Task AdicionarAsync(Agendamento agendamento, CancellationToken cancellationToken)
    {
        await dbContext.Agendamentos.AddAsync(agendamento, cancellationToken);
    }

    public Task AtualizarAsync(Agendamento agendamento, CancellationToken cancellationToken)
    {
        if (dbContext.Entry(agendamento).State == EntityState.Detached)
        {
            dbContext.Agendamentos.Update(agendamento);
        }

        return Task.CompletedTask;
    }

    public async Task<Agendamento?> BuscarPorIdAsync(Guid organizacaoId, Guid agendamentoId, CancellationToken cancellationToken)
    {
        return await dbContext.Agendamentos
            .FirstOrDefaultAsync(a => a.Id == agendamentoId && a.OrganizacaoId == organizacaoId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Agendamento>> BuscarAtivosSobrepostosAsync(
        Guid organizacaoId, Guid profissionalId, PeriodoAgendamento periodo, Guid? excluirAgendamentoId, CancellationToken cancellationToken)
    {
        var consulta = dbContext.Agendamentos.Where(a =>
            a.OrganizacaoId == organizacaoId
            && a.ProfissionalId == profissionalId
            && a.Status != StatusAgendamento.Cancelado
            && a.Periodo.InicioUtc < periodo.FimUtc
            && periodo.InicioUtc < a.Periodo.FimUtc);

        if (excluirAgendamentoId is not null)
        {
            consulta = consulta.Where(a => a.Id != excluirAgendamentoId.Value);
        }

        return await consulta.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Agendamento>> BuscarPorProfissionalEPeriodoAsync(
        Guid organizacaoId, Guid profissionalId, PeriodoAgendamento periodo, CancellationToken cancellationToken)
    {
        return await dbContext.Agendamentos
            .Where(a => a.OrganizacaoId == organizacaoId
                && a.ProfissionalId == profissionalId
                && a.Periodo.InicioUtc < periodo.FimUtc
                && periodo.InicioUtc < a.Periodo.FimUtc)
            .ToListAsync(cancellationToken);
    }
}
