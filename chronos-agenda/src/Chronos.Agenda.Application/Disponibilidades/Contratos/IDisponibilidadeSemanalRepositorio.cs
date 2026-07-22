using Chronos.Agenda.Domain.Disponibilidades.Entidades;

namespace Chronos.Agenda.Application.Disponibilidades.Contratos;

/// <summary>Acesso a disponibilidades semanais, restrito aos métodos que os casos de uso do MVP utilizam (sem CRUD
/// genérico).</summary>
public interface IDisponibilidadeSemanalRepositorio
{
    /// <summary>Persiste uma nova disponibilidade semanal.</summary>
    Task AdicionarAsync(DisponibilidadeSemanal disponibilidade, CancellationToken cancellationToken);

    /// <summary>Persiste o dia ou a janela atualizados de uma disponibilidade semanal.</summary>
    Task AtualizarAsync(DisponibilidadeSemanal disponibilidade, CancellationToken cancellationToken);

    /// <summary>Remove uma disponibilidade semanal.</summary>
    Task RemoverAsync(DisponibilidadeSemanal disponibilidade, CancellationToken cancellationToken);

    /// <summary>Busca as disponibilidades do profissional num dia da semana, dentro do limite da organização
    /// informada (RN01), para checar sobreposição ao criar ou alterar (Fase B item 8).</summary>
    Task<IReadOnlyCollection<DisponibilidadeSemanal>> BuscarPorProfissionalEDiaAsync(
        Guid organizacaoId, Guid profissionalId, DayOfWeek diaDaSemana, CancellationToken cancellationToken);

    /// <summary>Busca todas as disponibilidades do profissional, dentro do limite da organização informada (RN01),
    /// usadas para projetar a agenda (UC07).</summary>
    Task<IReadOnlyCollection<DisponibilidadeSemanal>> BuscarPorProfissionalAsync(
        Guid organizacaoId, Guid profissionalId, CancellationToken cancellationToken);
}
