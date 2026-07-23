using Chronos.Agenda.Application.Servicos.ListarServicos;

namespace Chronos.Agenda.Api.Endpoints.Servicos;

/// <summary>Endpoint de listagem dos serviços do catálogo de um profissional (UC03).</summary>
public sealed class ListarServicosEndpoint : IEndpoint
{
    public static void MapearEndpoint(IEndpointRouteBuilder rotas)
    {
        rotas.MapGet("/", ListarAsync)
            .WithName("ListarServicos")
            .WithSummary("Lista os serviços de um profissional")
            .WithDescription("Lista os serviços do catálogo de um profissional, com sua configuração comercial (UC03).")
            .Produces<IReadOnlyCollection<ServicoResumo>>(StatusCodes.Status200OK)
            .WithOrder(0);
    }

    private static async Task<IResult> ListarAsync(Guid profissionalId, ListarServicosHandler handler, CancellationToken cancellationToken)
    {
        var servicos = await handler.ExecutarAsync(new ListarServicosConsulta(profissionalId), cancellationToken);
        return Results.Ok(servicos);
    }
}
