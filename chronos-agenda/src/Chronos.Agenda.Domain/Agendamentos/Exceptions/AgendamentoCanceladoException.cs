using Chronos.Agenda.Domain.Compartilhado.Exceptions;

namespace Chronos.Agenda.Domain.Agendamentos.Exceptions;

/// <summary>Indica uma alteração tentada em um agendamento cancelado.</summary>
public sealed class AgendamentoCanceladoException()
    : DomainException("Um agendamento cancelado não pode ser alterado.");
