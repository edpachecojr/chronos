using Chronos.Agenda.Domain.Compartilhado.Exceptions;

namespace Chronos.Agenda.Domain.Usuarios.Exceptions;

/// <summary>Indica que um usuário não foi associado a uma organização válida.</summary>
public sealed class OrganizacaoUsuarioInvalidaException(Guid organizacaoId)
    : DomainException($"O usuário deve pertencer a uma organização válida; organização recebida: {organizacaoId}.");
