namespace Chronos.Agenda.Domain.Compartilhado.Exceptions;

/// <summary>Indica um nome próprio ausente ou fora do limite permitido.</summary>
public sealed class NomeInvalidoException(int comprimento)
    : DomainException($"O nome deve ter entre 1 e 120 caracteres; comprimento recebido: {comprimento}.");
