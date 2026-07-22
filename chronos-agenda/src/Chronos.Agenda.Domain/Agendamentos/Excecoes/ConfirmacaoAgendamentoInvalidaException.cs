using Chronos.Agenda.Domain.Agendamentos.Enumeracoes;
using Chronos.Agenda.Domain.Compartilhado.Excecoes;

namespace Chronos.Agenda.Domain.Agendamentos.Excecoes;

/// <summary>Indica uma confirmação incompatível com o estado atual do agendamento.</summary>
public sealed class ConfirmacaoAgendamentoInvalidaException(StatusAgendamento status)
    : DomainException($"Apenas agendamentos pendentes podem ser confirmados; estado atual: {status}.");
