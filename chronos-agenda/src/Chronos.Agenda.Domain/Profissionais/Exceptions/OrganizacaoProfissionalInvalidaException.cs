using Chronos.Agenda.Domain.Compartilhado.Exceptions;

namespace Chronos.Agenda.Domain.Profissionais.Exceptions;

/// <summary>Indica que um profissional não foi associado a uma organização válida.</summary>
public sealed class OrganizacaoProfissionalInvalidaException(Guid organizacaoId)
    : DomainException($"O profissional deve pertencer a uma organização válida; organização recebida: {organizacaoId}.");
