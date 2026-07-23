namespace Chronos.Agenda.Api.Endpoints.Organizacoes;

/// <summary>Corpo da requisição para configurar o perfil operacional da organização corrente. O endereço do
/// prestador é opcional: organizações somente-online não precisam de um endereço físico.</summary>
public sealed record ConfigurarPerfilOperacionalRequest(string? EnderecoPrestador, string FusoHorario);
