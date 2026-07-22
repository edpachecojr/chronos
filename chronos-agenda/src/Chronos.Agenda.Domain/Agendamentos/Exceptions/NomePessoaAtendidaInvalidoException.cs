using Chronos.Agenda.Domain.Compartilhado.Exceptions;

namespace Chronos.Agenda.Domain.Agendamentos.Exceptions;

/// <summary>Indica um nome inválido para a pessoa atendida.</summary>
public sealed class NomePessoaAtendidaInvalidoException(int comprimento)
    : DomainException($"O nome da pessoa atendida deve ter entre 1 e 120 caracteres; comprimento recebido: {comprimento}.");
