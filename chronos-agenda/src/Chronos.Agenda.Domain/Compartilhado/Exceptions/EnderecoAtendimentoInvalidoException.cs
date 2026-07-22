namespace Chronos.Agenda.Domain.Compartilhado.Exceptions;

/// <summary>Indica um endereço ausente ou fora do limite permitido.</summary>
public sealed class EnderecoAtendimentoInvalidoException(int comprimento)
    : DomainException($"O endereço do atendimento deve ter entre 1 e 300 caracteres; comprimento recebido: {comprimento}.");
