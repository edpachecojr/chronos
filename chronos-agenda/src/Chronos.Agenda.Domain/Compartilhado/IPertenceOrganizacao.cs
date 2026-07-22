namespace Chronos.Agenda.Domain.Compartilhado;

/// <summary>Expõe a organização proprietária de uma entidade multi-tenant.</summary>
public interface IPertenceOrganizacao
{
    Guid OrganizacaoId { get; }
}
