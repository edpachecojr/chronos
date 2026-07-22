namespace Chronos.Agenda.Application.Agendamentos.CancelarAgendamento;

/// <summary>Identifica o agendamento ativo a ser cancelado (UC06).</summary>
/// <example><code>var comando = new CancelarAgendamentoComando(agendamentoId);</code></example>
public sealed record CancelarAgendamentoComando(Guid AgendamentoId);
