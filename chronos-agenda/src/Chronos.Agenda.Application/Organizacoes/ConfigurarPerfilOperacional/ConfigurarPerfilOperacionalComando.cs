namespace Chronos.Agenda.Application.Organizacoes.ConfigurarPerfilOperacional;

/// <summary>
/// Dados para configurar o perfil operacional (endereço do prestador e fuso
/// horário) da organização corrente. <see cref="EnderecoPrestador"/> é
/// opcional: organizações somente-online não precisam de um endereço físico.
/// </summary>
/// <example><code>
/// var comando = new ConfigurarPerfilOperacionalComando("Av. Central, 20", "America/Sao_Paulo");
/// </code></example>
public sealed record ConfigurarPerfilOperacionalComando(string? EnderecoPrestador, string FusoHorario);
