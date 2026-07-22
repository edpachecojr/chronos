using Chronos.Agenda.Application.Profissionais.Contratos;
using Chronos.Agenda.Domain.Profissionais.Entidades;

namespace Chronos.Agenda.Application.Tests.Profissionais.Fakes;

internal sealed class FakeProfissionalRepositorio : IProfissionalRepositorio
{
    private readonly List<Profissional> _profissionais = [];

    public Task AdicionarAsync(Profissional profissional, CancellationToken cancellationToken)
    {
        _profissionais.Add(profissional);
        return Task.CompletedTask;
    }

    public Task<Profissional?> BuscarPorIdAsync(Guid organizacaoId, Guid profissionalId, CancellationToken cancellationToken)
    {
        var profissional = _profissionais
            .SingleOrDefault(p => p.Id == profissionalId && p.OrganizacaoId == organizacaoId);
        return Task.FromResult(profissional);
    }
}
