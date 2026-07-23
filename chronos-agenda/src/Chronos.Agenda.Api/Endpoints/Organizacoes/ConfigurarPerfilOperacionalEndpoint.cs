using Chronos.Agenda.Api.Erros;
using Chronos.Agenda.Application.Organizacoes.ConfigurarPerfilOperacional;

namespace Chronos.Agenda.Api.Endpoints.Organizacoes;

/// <summary>Endpoint que expõe <c>Organizacao.ConfigurarPerfilOperacional</c> à aplicação. Sem este endpoint, a
/// organização nunca tem fuso horário configurado em produção e UC04/UC05/UC07 falham sempre com 409 (ver
/// docs/backlog/roadmap-casos-de-uso.md, bloqueador crítico).</summary>
public sealed class ConfigurarPerfilOperacionalEndpoint : IEndpoint
{
    public static void MapearEndpoint(IEndpointRouteBuilder rotas)
    {
        rotas.MapPut("/perfil-operacional", ConfigurarAsync)
            .WithName("ConfigurarPerfilOperacional")
            .WithSummary("Configura o perfil operacional da organização")
            .WithDescription("Define o endereço do prestador e o fuso horário IANA usados para calcular local e disponibilidade dos agendamentos (UC04, UC05, UC07).")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOrder(2);
    }

    private static async Task<IResult> ConfigurarAsync(
        ConfigurarPerfilOperacionalRequest requisicao, ConfigurarPerfilOperacionalHandler handler, CancellationToken cancellationToken)
    {
        var comando = new ConfigurarPerfilOperacionalComando(requisicao.EnderecoPrestador, requisicao.FusoHorario);
        var resultado = await handler.ExecutarAsync(comando, cancellationToken);
        return resultado.ParaHttpResultado();
    }
}
