namespace Chronos.Agenda.Application.Disponibilidades.AlterarDisponibilidade;

/// <summary>Dados para alterar o dia ou a janela de uma disponibilidade semanal já configurada (UC02).</summary>
/// <example><code>
/// var comando = new AlterarDisponibilidadeComando(
///     profissionalId, disponibilidadeId, DayOfWeek.Tuesday, new TimeOnly(14, 0), new TimeOnly(18, 0));
/// </code></example>
public sealed record AlterarDisponibilidadeComando(
    Guid ProfissionalId, Guid DisponibilidadeId, DayOfWeek DiaDaSemana, TimeOnly Inicio, TimeOnly Fim);
