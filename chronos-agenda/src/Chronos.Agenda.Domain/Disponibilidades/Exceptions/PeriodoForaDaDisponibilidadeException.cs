using Chronos.Agenda.Domain.Compartilhado.Exceptions;

namespace Chronos.Agenda.Domain.Disponibilidades.Exceptions;

/// <summary>Indica que o período do agendamento, no fuso da organização, não está contido em nenhuma disponibilidade do profissional.</summary>
public sealed class PeriodoForaDaDisponibilidadeException(DateTime inicioUtc, DateTime fimUtc)
    : DomainException($"O período do agendamento (início: {inicioUtc:O}, fim: {fimUtc:O}) não está dentro da disponibilidade do profissional.");
