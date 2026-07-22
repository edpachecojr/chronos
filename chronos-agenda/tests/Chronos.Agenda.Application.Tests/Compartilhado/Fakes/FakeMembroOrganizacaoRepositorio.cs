using Chronos.Agenda.Application.Compartilhado.Contratos;

namespace Chronos.Agenda.Application.Tests.Compartilhado.Fakes;

internal sealed class FakeMembroOrganizacaoRepositorio : IMembroOrganizacaoRepositorio
{
    private readonly Dictionary<Guid, Guid> _organizacaoPorUsuario = [];

    public Task AdicionarAsync(Guid usuarioId, Guid organizacaoId, CancellationToken cancellationToken)
    {
        _organizacaoPorUsuario[usuarioId] = organizacaoId;
        return Task.CompletedTask;
    }

    public Task<Guid?> BuscarOrganizacaoIdDoUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken)
    {
        var encontrado = _organizacaoPorUsuario.TryGetValue(usuarioId, out var organizacaoId);
        return Task.FromResult(encontrado ? organizacaoId : (Guid?)null);
    }
}
