using Chronos.Agenda.Domain.Compartilhado.Excecoes;

namespace Chronos.Agenda.Domain.Servicos.Excecoes;

/// <summary>Indica uma duração de serviço fora do intervalo permitido.</summary>
public sealed class DuracaoServicoInvalidaException(TimeSpan valor)
    : DomainException($"A duração do serviço deve ser maior que zero e de no máximo 12 horas; duração recebida: {valor}.");
