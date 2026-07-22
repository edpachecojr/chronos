using Chronos.Agenda.Application.Disponibilidades.Contratos;
using Chronos.Agenda.Domain.Disponibilidades.Entidades;

namespace Chronos.Agenda.Application.Tests.Disponibilidades.Fakes;

internal sealed class FakeDisponibilidadeSemanalRepositorio : IDisponibilidadeSemanalRepositorio
{
    private readonly List<DisponibilidadeSemanal> _disponibilidades = [];

    public Task AdicionarAsync(DisponibilidadeSemanal disponibilidade, CancellationToken cancellationToken)
    {
        _disponibilidades.Add(disponibilidade);
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(DisponibilidadeSemanal disponibilidade, CancellationToken cancellationToken)
    {
        var indice = _disponibilidades.FindIndex(d => d.Id == disponibilidade.Id);
        _disponibilidades[indice] = disponibilidade;
        return Task.CompletedTask;
    }

    public Task RemoverAsync(DisponibilidadeSemanal disponibilidade, CancellationToken cancellationToken)
    {
        _disponibilidades.RemoveAll(d => d.Id == disponibilidade.Id);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<DisponibilidadeSemanal>> BuscarPorProfissionalEDiaAsync(
        Guid organizacaoId, Guid profissionalId, DayOfWeek diaDaSemana, CancellationToken cancellationToken)
    {
        var encontradas = Filtrar(organizacaoId, profissionalId)
            .Where(d => d.DiaDaSemana == diaDaSemana)
            .ToList();
        return Task.FromResult<IReadOnlyCollection<DisponibilidadeSemanal>>(encontradas);
    }

    public Task<IReadOnlyCollection<DisponibilidadeSemanal>> BuscarPorProfissionalAsync(
        Guid organizacaoId, Guid profissionalId, CancellationToken cancellationToken)
    {
        var encontradas = Filtrar(organizacaoId, profissionalId).ToList();
        return Task.FromResult<IReadOnlyCollection<DisponibilidadeSemanal>>(encontradas);
    }

    private IEnumerable<DisponibilidadeSemanal> Filtrar(Guid organizacaoId, Guid profissionalId)
    {
        return _disponibilidades.Where(d => d.OrganizacaoId == organizacaoId && d.ProfissionalId == profissionalId);
    }
}
