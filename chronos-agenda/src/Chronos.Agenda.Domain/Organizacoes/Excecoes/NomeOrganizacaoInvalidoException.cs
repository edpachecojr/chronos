using Chronos.Agenda.Domain.Compartilhado.Excecoes;

namespace Chronos.Agenda.Domain.Organizacoes.Excecoes;

/// <summary>Indica um nome de organização ausente ou fora do limite permitido.</summary>
public sealed class NomeOrganizacaoInvalidoException(int comprimento)
    : DomainException($"O nome da organização deve ter entre 1 e 120 caracteres; comprimento recebido: {comprimento}.");
