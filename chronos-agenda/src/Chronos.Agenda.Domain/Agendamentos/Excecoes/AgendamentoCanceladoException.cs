using Chronos.Agenda.Domain.Compartilhado.Excecoes;

namespace Chronos.Agenda.Domain.Agendamentos.Excecoes;

/// <summary>Indica uma alteração tentada em um agendamento cancelado.</summary>
public sealed class AgendamentoCanceladoException()
    : DomainException("Um agendamento cancelado não pode ser alterado.");
