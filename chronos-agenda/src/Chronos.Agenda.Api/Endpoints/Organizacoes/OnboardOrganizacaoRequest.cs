namespace Chronos.Agenda.Api.Endpoints.Organizacoes;

/// <summary>Corpo da requisição de onboarding de uma organização (UC01). O usuário autenticado é obtido do
/// contexto da requisição, não do corpo.</summary>
public sealed record OnboardOrganizacaoRequest(string Nome, string NomeProfissionalInicial);
