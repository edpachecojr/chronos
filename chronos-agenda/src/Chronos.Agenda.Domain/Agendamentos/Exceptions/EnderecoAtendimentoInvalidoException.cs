using Chronos.Agenda.Domain.Compartilhado.Exceptions;

namespace Chronos.Agenda.Domain.Agendamentos.Exceptions;

/// <summary>Indica um endereço ausente ou fora do limite permitido.</summary>
public sealed class EnderecoAtendimentoInvalidoException(int comprimento)
    : DomainException($"O endereço do atendimento deve ter entre 1 e 300 caracteres; comprimento recebido: {comprimento}.");
