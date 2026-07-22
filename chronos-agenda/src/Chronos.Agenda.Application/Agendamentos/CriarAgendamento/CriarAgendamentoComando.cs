using Chronos.Agenda.Application.Agendamentos;
using Chronos.Agenda.Domain.Agendamentos.Enums;

namespace Chronos.Agenda.Application.Agendamentos.CriarAgendamento;

/// <summary>
/// Dados para criar um novo agendamento (UC04). <see cref="Inicio"/> carrega o
/// offset explícito informado pelo cliente (ISO 8601 com offset); a conversão
/// para UTC é direta e nunca ambígua (ADR 0005).
/// </summary>
/// <example><code>
/// var comando = new CriarAgendamentoComando(
///     profissionalId, servicoId, "Maria Silva", TipoPessoaAtendida.Paciente,
///     new DateTimeOffset(2026, 7, 27, 10, 0, 0, TimeSpan.FromHours(-3)), null);
/// </code></example>
public sealed record CriarAgendamentoComando(
    Guid ProfissionalId,
    Guid ServicoId,
    string NomePessoaAtendida,
    TipoPessoaAtendida TipoPessoaAtendida,
    DateTimeOffset Inicio,
    string? EnderecoPessoaAtendida) : IDadosAgendamento;
