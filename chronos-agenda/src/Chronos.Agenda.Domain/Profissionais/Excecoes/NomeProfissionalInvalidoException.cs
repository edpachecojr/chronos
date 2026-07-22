using Chronos.Agenda.Domain.Compartilhado.Excecoes;

namespace Chronos.Agenda.Domain.Profissionais.Excecoes;

/// <summary>Indica um nome de profissional ausente ou fora do limite permitido.</summary>
public sealed class NomeProfissionalInvalidoException(int comprimento)
    : DomainException($"O nome do profissional deve ter entre 1 e 120 caracteres; comprimento recebido: {comprimento}.");
