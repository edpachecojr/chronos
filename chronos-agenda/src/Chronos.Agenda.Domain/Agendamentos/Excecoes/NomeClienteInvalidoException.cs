using Chronos.Agenda.Domain.Compartilhado.Excecoes;

namespace Chronos.Agenda.Domain.Agendamentos.Excecoes;

/// <summary>Indica um nome de cliente ausente ou fora do limite permitido.</summary>
public sealed class NomeClienteInvalidoException(int comprimento)
    : DomainException($"O nome do cliente deve ter entre 1 e 120 caracteres; comprimento recebido: {comprimento}.");
