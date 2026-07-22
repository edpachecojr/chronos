using Chronos.Agenda.Domain.Disponibilidades.ObjetosValor;

namespace Chronos.Agenda.Application.Agendamentos;

/// <summary>Projeção somente leitura da agenda de um profissional em um dia local: as janelas de disponibilidade
/// configuradas e os períodos já ocupados por agendamentos ativos, ambos no fuso horário da organização (UC07).
/// Compartilhado entre a consulta diária e a consulta semanal, que agrega um <see cref="AgendaDiariaResultado"/>
/// por dia.</summary>
public sealed record AgendaDiariaResultado(
    DateOnly Data,
    DayOfWeek DiaDaSemana,
    IReadOnlyCollection<JanelaHorario> JanelasDisponiveis,
    IReadOnlyCollection<PeriodoOcupado> PeriodosOcupados);
