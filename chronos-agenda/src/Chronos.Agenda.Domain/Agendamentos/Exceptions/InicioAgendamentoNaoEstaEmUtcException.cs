using Chronos.Agenda.Domain.Compartilhado.Exceptions;

namespace Chronos.Agenda.Domain.Agendamentos.Exceptions;

/// <summary>Indica que o início de um agendamento não foi informado em UTC.</summary>
public sealed class InicioAgendamentoNaoEstaEmUtcException(DateTimeKind tipo)
    : DomainException($"O início do agendamento deve estar em UTC; tipo recebido: {tipo}.");
