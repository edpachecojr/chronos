using Chronos.Agenda.Application.Agendamentos.Contratos;
using Chronos.Agenda.Domain.Agendamentos.Entidades;
using Chronos.Agenda.Domain.Agendamentos.Enums;
using Chronos.Agenda.Domain.Agendamentos.ObjetosValor;

namespace Chronos.Agenda.Application.Tests.Agendamentos.Fakes;

internal sealed class FakeAgendamentoRepositorio : IAgendamentoRepositorio
{
    private readonly List<Agendamento> _agendamentos = [];

    public Task AdicionarAsync(Agendamento agendamento, CancellationToken cancellationToken)
    {
        _agendamentos.Add(agendamento);
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(Agendamento agendamento, CancellationToken cancellationToken)
    {
        var indice = _agendamentos.FindIndex(a => a.Id == agendamento.Id);
        _agendamentos[indice] = agendamento;
        return Task.CompletedTask;
    }

    public Task<Agendamento?> BuscarPorIdAsync(Guid organizacaoId, Guid agendamentoId, CancellationToken cancellationToken)
    {
        var agendamento = Filtrar(organizacaoId).SingleOrDefault(a => a.Id == agendamentoId);
        return Task.FromResult(agendamento);
    }

    public Task<IReadOnlyCollection<Agendamento>> BuscarAtivosSobrepostosAsync(
        Guid organizacaoId, Guid profissionalId, PeriodoAgendamento periodo, Guid? excluirAgendamentoId, CancellationToken cancellationToken)
    {
        var encontrados = Filtrar(organizacaoId)
            .Where(a => a.ProfissionalId == profissionalId
                && a.Status != StatusAgendamento.Cancelado
                && a.Id != excluirAgendamentoId
                && a.Periodo.Sobrepoe(periodo))
            .ToList();
        return Task.FromResult<IReadOnlyCollection<Agendamento>>(encontrados);
    }

    public Task<IReadOnlyCollection<Agendamento>> BuscarPorProfissionalEPeriodoAsync(
        Guid organizacaoId, Guid profissionalId, PeriodoAgendamento periodo, CancellationToken cancellationToken)
    {
        var encontrados = Filtrar(organizacaoId)
            .Where(a => a.ProfissionalId == profissionalId && a.Periodo.Sobrepoe(periodo))
            .ToList();
        return Task.FromResult<IReadOnlyCollection<Agendamento>>(encontrados);
    }

    private IEnumerable<Agendamento> Filtrar(Guid organizacaoId)
    {
        return _agendamentos.Where(a => a.OrganizacaoId == organizacaoId);
    }
}
