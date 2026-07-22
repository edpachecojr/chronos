namespace Chronos.Agenda.Application.Organizacoes.CriarOrganizacao;

/// <summary>Identificadores produzidos pelo onboarding de uma organização (UC01).</summary>
public sealed record CriarOrganizacaoResultado(Guid OrganizacaoId, Guid ProfissionalId);
