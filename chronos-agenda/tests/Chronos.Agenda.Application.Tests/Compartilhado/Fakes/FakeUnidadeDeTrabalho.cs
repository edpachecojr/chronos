using Chronos.Agenda.Application.Compartilhado.Contratos;

namespace Chronos.Agenda.Application.Tests.Compartilhado.Fakes;

internal sealed class FakeUnidadeDeTrabalho : IUnidadeDeTrabalho
{
    public int VezesSalvo { get; private set; }

    public Task SalvarAlteracoesAsync(CancellationToken cancellationToken)
    {
        VezesSalvo++;
        return Task.CompletedTask;
    }
}
