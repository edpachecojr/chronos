namespace Chronos.Agenda.Application.Disponibilidades.ListarDisponibilidades;

/// <summary>Projeção somente leitura de uma disponibilidade semanal configurada (UC02).</summary>
public sealed record DisponibilidadeResumo(Guid DisponibilidadeId, DayOfWeek DiaDaSemana, TimeOnly Inicio, TimeOnly Fim);
