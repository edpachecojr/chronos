using Chronos.Agenda.Domain.Compartilhado.Exceptions;

namespace Chronos.Agenda.Domain.Disponibilidades.Exceptions;

/// <summary>Indica uma janela de atendimento cujo fim não sucede o início.</summary>
public sealed class JanelaHorarioInvalidaException(TimeOnly inicio, TimeOnly fim)
    : DomainException($"O fim da janela de atendimento ({fim}) deve ser posterior ao início ({inicio}).");
