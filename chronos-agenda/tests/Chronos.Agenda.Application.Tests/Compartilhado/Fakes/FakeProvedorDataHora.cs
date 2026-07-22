using Chronos.Agenda.Domain.Compartilhado.Contratos;

namespace Chronos.Agenda.Application.Tests.Compartilhado.Fakes;

internal sealed class FakeProvedorDataHora(DateTime utcNow) : IProvedorDataHora
{
    public DateTime UtcNow { get; set; } = utcNow;
}
