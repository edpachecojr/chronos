namespace Chronos.Agenda.Application.Organizacoes.ConsultarOrganizacaoAtual;

/// <summary>Identificação da organização vinculada ao usuário autenticado.</summary>
public sealed record ConsultarOrganizacaoAtualResultado(Guid OrganizacaoId, string Nome);
