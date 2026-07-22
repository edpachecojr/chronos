namespace Chronos.Agenda.Application.Agendamentos.ConsultarAgendaSemanal;

/// <summary>Filtro de leitura para projetar a ocupação da agenda de um profissional nos sete dias locais a partir
/// de <see cref="InicioDaSemana"/> (UC07).</summary>
/// <example><code>var consulta = new ConsultarAgendaSemanalConsulta(profissionalId, new DateOnly(2026, 7, 27));</code></example>
public sealed record ConsultarAgendaSemanalConsulta(Guid ProfissionalId, DateOnly InicioDaSemana);
