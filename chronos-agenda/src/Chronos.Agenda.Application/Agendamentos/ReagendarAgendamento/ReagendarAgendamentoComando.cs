using Chronos.Agenda.Application.Agendamentos;
using Chronos.Agenda.Domain.Agendamentos.Enums;

namespace Chronos.Agenda.Application.Agendamentos.ReagendarAgendamento;

/// <summary>
/// Dados para reagendar ou editar um agendamento existente (UC05). Compartilha o mesmo formato de entrada de
/// <see cref="CriarAgendamento.CriarAgendamentoComando"/> (mesma preparação e validação, via
/// <see cref="AgendamentoPreparador"/>), mas <see cref="ProfissionalId"/> e <see cref="ServicoId"/> devem
/// corresponder ao profissional e ao serviço já vinculados ao agendamento — eles não podem ser trocados, pois são
/// imutáveis em <c>Agendamento</c> após a criação.
/// </summary>
/// <example><code>
/// var comando = new ReagendarAgendamentoComando(
///     agendamentoId, profissionalId, servicoId, "Maria Silva", TipoPessoaAtendida.Paciente,
///     new DateTimeOffset(2026, 7, 28, 14, 0, 0, TimeSpan.FromHours(-3)), null);
/// </code></example>
public sealed record ReagendarAgendamentoComando(
    Guid AgendamentoId,
    Guid ProfissionalId,
    Guid ServicoId,
    string NomePessoaAtendida,
    TipoPessoaAtendida TipoPessoaAtendida,
    DateTimeOffset Inicio,
    string? EnderecoPessoaAtendida) : IDadosAgendamento;
