using Chronos.Agenda.Domain.Agendamentos.Enums;

namespace Chronos.Agenda.Api.Endpoints.Agendamentos;

/// <summary>Corpo da requisição para reagendar ou editar um agendamento existente (UC05). O identificador do
/// agendamento vem da rota, não do corpo.</summary>
public sealed record ReagendarAgendamentoRequest(
    Guid ProfissionalId,
    Guid ServicoId,
    string NomePessoaAtendida,
    TipoPessoaAtendida TipoPessoaAtendida,
    DateTimeOffset Inicio,
    string? EnderecoPessoaAtendida);
