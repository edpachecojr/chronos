namespace Chronos.Agenda.Application.Agendamentos.ConfirmarAgendamento;

/// <summary>Identifica o agendamento pendente a ser confirmado (UC06).</summary>
/// <example><code>var comando = new ConfirmarAgendamentoComando(agendamentoId);</code></example>
public sealed record ConfirmarAgendamentoComando(Guid AgendamentoId);
