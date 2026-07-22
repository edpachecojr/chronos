using Chronos.Agenda.Domain.Compartilhado.Exceptions;

namespace Chronos.Agenda.Domain.Agendamentos.Exceptions;

/// <summary>Indica que o fim do período não é posterior ao início.</summary>
public sealed class FimAgendamentoInvalidoException(DateTime inicioUtc, DateTime fimUtc)
    : DomainException($"O fim do agendamento deve ser posterior ao início; início: {inicioUtc:O}, fim: {fimUtc:O}.");
