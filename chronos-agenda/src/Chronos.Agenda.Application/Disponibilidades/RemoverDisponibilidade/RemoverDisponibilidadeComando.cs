namespace Chronos.Agenda.Application.Disponibilidades.RemoverDisponibilidade;

/// <summary>Dados para remover uma disponibilidade semanal de um profissional (UC02).</summary>
/// <example><code>var comando = new RemoverDisponibilidadeComando(profissionalId, disponibilidadeId);</code></example>
public sealed record RemoverDisponibilidadeComando(Guid ProfissionalId, Guid DisponibilidadeId);
