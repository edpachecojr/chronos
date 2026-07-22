using Chronos.Agenda.Domain.Compartilhado.Exceptions;

namespace Chronos.Agenda.Domain.Agendamentos.Exceptions;

/// <summary>Indica que uma data e hora do período não foi informada em UTC.</summary>
public sealed class DataHoraAgendamentoNaoEstaEmUtcException(string parametro, DateTimeKind tipo)
    : DomainException($"{parametro} do agendamento deve estar em UTC; tipo recebido: {tipo}.");
