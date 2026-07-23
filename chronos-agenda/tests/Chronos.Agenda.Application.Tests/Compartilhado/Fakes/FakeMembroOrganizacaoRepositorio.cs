using Chronos.Agenda.Application.Compartilhado;
using Chronos.Agenda.Application.Compartilhado.Contratos;

namespace Chronos.Agenda.Application.Tests.Compartilhado.Fakes;

internal sealed class FakeMembroOrganizacaoRepositorio : IMembroOrganizacaoRepositorio
{
    private readonly Dictionary<Guid, (Guid OrganizacaoId, PapelMembroOrganizacao Papel)> _vinculoPorUsuario = [];

    public Task AdicionarAsync(Guid usuarioId, Guid organizacaoId, PapelMembroOrganizacao papel, CancellationToken cancellationToken)
    {
        _vinculoPorUsuario[usuarioId] = (organizacaoId, papel);
        return Task.CompletedTask;
    }

    public Task<Guid?> BuscarOrganizacaoIdDoUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken)
    {
        var encontrado = _vinculoPorUsuario.TryGetValue(usuarioId, out var vinculo);
        return Task.FromResult(encontrado ? vinculo.OrganizacaoId : (Guid?)null);
    }

    public Task<PapelMembroOrganizacao?> BuscarPapelDoUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken)
    {
        var encontrado = _vinculoPorUsuario.TryGetValue(usuarioId, out var vinculo);
        return Task.FromResult(encontrado ? vinculo.Papel : (PapelMembroOrganizacao?)null);
    }
}
