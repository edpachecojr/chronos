namespace Chronos.Agenda.Application.Agendamentos.ConsultarAgendaDiaria;

/// <summary>Filtro de leitura para projetar a ocupação da agenda de um profissional em um único dia local (UC07).</summary>
/// <example><code>var consulta = new ConsultarAgendaDiariaConsulta(profissionalId, new DateOnly(2026, 7, 27));</code></example>
public sealed record ConsultarAgendaDiariaConsulta(Guid ProfissionalId, DateOnly Data);
