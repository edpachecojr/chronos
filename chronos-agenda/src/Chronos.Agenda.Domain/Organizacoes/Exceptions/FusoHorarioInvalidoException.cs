using Chronos.Agenda.Domain.Compartilhado.Exceptions;

namespace Chronos.Agenda.Domain.Organizacoes.Exceptions;

/// <summary>Indica um identificador de fuso horário IANA ausente ou desconhecido.</summary>
public sealed class FusoHorarioInvalidoException(string identificador)
    : DomainException($"O fuso horário deve ser um identificador IANA reconhecido; identificador recebido: '{identificador}'.");
