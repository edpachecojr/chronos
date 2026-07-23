using Chronos.Agenda.Application.Organizacoes.ConsultarOrganizacaoAtual;

namespace Chronos.Agenda.Api.Endpoints.Organizacoes;

/// <summary>Endpoint de consulta da organização do usuário autenticado, usado pelo frontend para decidir entre
/// encaminhar ao onboarding (UC01) ou ao produto.</summary>
public sealed class ConsultarOrganizacaoAtualEndpoint : IEndpoint
{
    public static void MapearEndpoint(IEndpointRouteBuilder rotas)
    {
        rotas.MapGet("/atual", ConsultarAsync)
            .WithName("ConsultarOrganizacaoAtual")
            .WithSummary("Consulta a organização do usuário autenticado")
            .WithDescription("Retorna a organização vinculada ao usuário autenticado, ou 204 caso o onboarding (UC01) ainda não tenha sido concluído.")
            .Produces<ConsultarOrganizacaoAtualResultado>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .WithOrder(0);
    }

    private static async Task<IResult> ConsultarAsync(
        ConsultarOrganizacaoAtualHandler handler, CancellationToken cancellationToken)
    {
        var resultado = await handler.ExecutarAsync(cancellationToken);
        return resultado is null ? Results.NoContent() : Results.Ok(resultado);
    }
}
