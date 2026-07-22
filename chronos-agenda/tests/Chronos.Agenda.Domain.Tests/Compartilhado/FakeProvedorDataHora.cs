using Chronos.Agenda.Domain.Compartilhado.Contratos;

namespace Chronos.Agenda.Domain.Tests.Compartilhado;

internal sealed class FakeProvedorDataHora(DateTime utcNow) : IProvedorDataHora
{
    public DateTime UtcNow { get; set; } = utcNow;
}
