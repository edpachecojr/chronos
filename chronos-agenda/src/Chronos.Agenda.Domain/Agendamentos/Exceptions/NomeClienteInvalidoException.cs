using Chronos.Agenda.Domain.Compartilhado.Exceptions;

namespace Chronos.Agenda.Domain.Agendamentos.Exceptions;

/// <summary>Indica um nome de cliente ausente ou fora do limite permitido.</summary>
public sealed class NomeClienteInvalidoException(int comprimento)
    : DomainException($"O nome do cliente deve ter entre 1 e 120 caracteres; comprimento recebido: {comprimento}.");
