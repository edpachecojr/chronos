using Chronos.Agenda.Application.Agendamentos;

namespace Chronos.Agenda.Application.Agendamentos.ConsultarAgendaSemanal;

/// <summary>Agenda de um profissional nos sete dias locais consultados (UC07), um <see cref="AgendaDiariaResultado"/>
/// por dia, na ordem de <see cref="ConsultarAgendaSemanalConsulta.InicioDaSemana"/>.</summary>
public sealed record AgendaSemanalResultado(IReadOnlyCollection<AgendaDiariaResultado> Dias);
