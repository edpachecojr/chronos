using Chronos.Agenda.Application.Profissionais.ListarProfissionais;

namespace Chronos.Agenda.Api.Endpoints.Profissionais;

/// <summary>Endpoint de listagem dos profissionais vinculados à organização corrente.</summary>
public sealed class ListarProfissionaisEndpoint : IEndpoint
{
    public static void MapearEndpoint(IEndpointRouteBuilder rotas)
    {
        rotas.MapGet("/", ListarAsync)
            .WithName("ListarProfissionais")
            .WithSummary("Lista os profissionais da organização")
            .WithDescription("Lista os profissionais vinculados à organização corrente do usuário autenticado.")
            .Produces<IReadOnlyCollection<ProfissionalResumo>>(StatusCodes.Status200OK)
            .WithOrder(0);
    }

    private static async Task<IResult> ListarAsync(ListarProfissionaisHandler handler, CancellationToken cancellationToken)
    {
        var profissionais = await handler.ExecutarAsync(cancellationToken);
        return Results.Ok(profissionais);
    }
}
