using Chronos.Agenda.Api.Erros;
using Chronos.Agenda.Application.Disponibilidades.RemoverDisponibilidade;

namespace Chronos.Agenda.Api.Endpoints.Disponibilidades;

/// <summary>Endpoint de remoção de uma janela de disponibilidade semanal existente (UC02).</summary>
public sealed class RemoverDisponibilidadeEndpoint : IEndpoint
{
    public static void MapearEndpoint(IEndpointRouteBuilder rotas)
    {
        rotas.MapDelete("/{disponibilidadeId:guid}", RemoverAsync)
            .WithName("RemoverDisponibilidade")
            .WithSummary("Remove uma disponibilidade")
            .WithDescription("Remove uma janela de disponibilidade semanal existente do profissional (UC02).")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOrder(3);
    }

    private static async Task<IResult> RemoverAsync(
        Guid disponibilidadeId, Guid profissionalId, RemoverDisponibilidadeHandler handler, CancellationToken cancellationToken)
    {
        var comando = new RemoverDisponibilidadeComando(profissionalId, disponibilidadeId);
        var resultado = await handler.ExecutarAsync(comando, cancellationToken);
        return resultado.ParaHttpResultado();
    }
}
