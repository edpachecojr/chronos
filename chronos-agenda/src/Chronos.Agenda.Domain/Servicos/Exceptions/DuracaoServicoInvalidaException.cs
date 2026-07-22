using Chronos.Agenda.Domain.Compartilhado.Exceptions;

namespace Chronos.Agenda.Domain.Servicos.Exceptions;

/// <summary>Indica uma duração de serviço fora do intervalo permitido.</summary>
public sealed class DuracaoServicoInvalidaException(TimeSpan valor)
    : DomainException($"A duração do serviço deve ser maior que zero e de no máximo 12 horas; duração recebida: {valor}.");
