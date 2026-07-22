using Chronos.Agenda.Domain.Compartilhado.Contratos;

namespace Chronos.Agenda.Infrastructure.Compartilhado;

/// <summary>Fornece o instante UTC atual do relógio do sistema.</summary>
public sealed class ProvedorDataHoraUtc : IProvedorDataHora
{
    public DateTime UtcNow => DateTime.UtcNow;
}
