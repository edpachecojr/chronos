namespace Chronos.Agenda.Application.Disponibilidades.CriarDisponibilidade;

/// <summary>Dados para configurar uma nova janela semanal de atendimento de um profissional (UC02).</summary>
/// <example><code>
/// var comando = new CriarDisponibilidadeComando(
///     profissionalId, DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(12, 0));
/// </code></example>
public sealed record CriarDisponibilidadeComando(Guid ProfissionalId, DayOfWeek DiaDaSemana, TimeOnly Inicio, TimeOnly Fim);
