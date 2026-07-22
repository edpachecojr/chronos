using Chronos.Agenda.Domain.Compartilhado.Exceptions;

namespace Chronos.Agenda.Domain.Servicos.Exceptions;

/// <summary>Indica um preço negativo ou com precisão monetária inválida.</summary>
public sealed class PrecoServicoInvalidoException(decimal valor)
    : DomainException($"O preço do serviço deve ser não negativo e ter no máximo duas casas decimais; preço recebido: {valor}.");
