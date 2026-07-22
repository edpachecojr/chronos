using Chronos.Agenda.Application.Organizacoes.Contratos;
using Chronos.Agenda.Domain.Organizacoes.Entidades;

namespace Chronos.Agenda.Application.Tests.Organizacoes.Fakes;

internal sealed class FakeOrganizacaoRepositorio : IOrganizacaoRepositorio
{
    private readonly List<Organizacao> _organizacoes = [];

    public Task AdicionarAsync(Organizacao organizacao, CancellationToken cancellationToken)
    {
        _organizacoes.Add(organizacao);
        return Task.CompletedTask;
    }

    public Task<Organizacao?> BuscarPorIdAsync(Guid organizacaoId, CancellationToken cancellationToken)
    {
        var organizacao = _organizacoes.SingleOrDefault(o => o.Id == organizacaoId);
        return Task.FromResult(organizacao);
    }
}
