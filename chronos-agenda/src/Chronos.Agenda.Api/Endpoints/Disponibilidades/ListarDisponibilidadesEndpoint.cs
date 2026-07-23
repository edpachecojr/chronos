using Chronos.Agenda.Application.Disponibilidades.ListarDisponibilidades;

namespace Chronos.Agenda.Api.Endpoints.Disponibilidades;

/// <summary>Endpoint de listagem das disponibilidades semanais configuradas de um profissional (UC02).</summary>
public sealed class ListarDisponibilidadesEndpoint : IEndpoint
{
    public static void MapearEndpoint(IEndpointRouteBuilder rotas)
    {
        rotas.MapGet("/", ListarAsync)
            .WithName("ListarDisponibilidades")
            .WithSummary("Lista as disponibilidades de um profissional")
            .WithDescription("Lista as disponibilidades semanais configuradas de um profissional (UC02).")
            .Produces<IReadOnlyCollection<DisponibilidadeResumo>>(StatusCodes.Status200OK)
            .WithOrder(0);
    }

    private static async Task<IResult> ListarAsync(Guid profissionalId, ListarDisponibilidadesHandler handler, CancellationToken cancellationToken)
    {
        var disponibilidades = await handler.ExecutarAsync(new ListarDisponibilidadesConsulta(profissionalId), cancellationToken);
        return Results.Ok(disponibilidades);
    }
}
