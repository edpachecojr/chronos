namespace Chronos.Agenda.Domain.Compartilhado.Contratos;

/// <summary>Fornece o instante UTC atual para operações do domínio.</summary>
public interface IProvedorDataHora
{
    DateTime UtcNow { get; }
}
