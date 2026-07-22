using Chronos.Agenda.Domain.Agendamentos.Enums;
using Chronos.Agenda.Domain.Compartilhado.Exceptions;

namespace Chronos.Agenda.Domain.Agendamentos.Exceptions;

/// <summary>Indica uma confirmação incompatível com o estado atual do agendamento.</summary>
public sealed class ConfirmacaoAgendamentoInvalidaException(StatusAgendamento status)
    : DomainException($"Apenas agendamentos pendentes podem ser confirmados; estado atual: {status}.");
