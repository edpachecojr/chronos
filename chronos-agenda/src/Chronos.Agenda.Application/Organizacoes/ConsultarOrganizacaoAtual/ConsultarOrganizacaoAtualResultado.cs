namespace Chronos.Agenda.Application.Organizacoes.ConsultarOrganizacaoAtual;

/// <summary>Identificação e perfil operacional da organização vinculada ao usuário autenticado.
/// <see cref="EnderecoPrestador"/>/<see cref="FusoHorario"/> são nulos até o perfil operacional ser configurado
/// (ver <c>ConfigurarPerfilOperacional</c>, UC04).</summary>
public sealed record ConsultarOrganizacaoAtualResultado(Guid OrganizacaoId, string Nome, string? EnderecoPrestador, string? FusoHorario);
