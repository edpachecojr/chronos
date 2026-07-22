using Chronos.Agenda.Application.Servicos.Contratos;
using Chronos.Agenda.Domain.Servicos.Entidades;

namespace Chronos.Agenda.Application.Tests.Servicos.Fakes;

internal sealed class FakeServicoRepositorio : IServicoRepositorio
{
    private readonly List<Servico> _servicos = [];

    public Task AdicionarAsync(Servico servico, CancellationToken cancellationToken)
    {
        _servicos.Add(servico);
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(Servico servico, CancellationToken cancellationToken)
    {
        var indice = _servicos.FindIndex(s => s.Id == servico.Id);
        _servicos[indice] = servico;
        return Task.CompletedTask;
    }

    public Task<Servico?> BuscarPorIdAsync(Guid organizacaoId, Guid servicoId, CancellationToken cancellationToken)
    {
        var servico = _servicos
            .SingleOrDefault(s => s.Id == servicoId && s.OrganizacaoId == organizacaoId);
        return Task.FromResult(servico);
    }
}
