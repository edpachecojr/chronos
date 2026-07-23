using Chronos.Agenda.Api.Erros;
using Chronos.Agenda.Application.Agendamentos.CancelarAgendamento;

namespace Chronos.Agenda.Api.Endpoints.Agendamentos;

/// <summary>Endpoint de cancelamento de um agendamento existente (UC06).</summary>
public sealed class CancelarAgendamentoEndpoint : IEndpoint
{
    public static void MapearEndpoint(IEndpointRouteBuilder rotas)
    {
        rotas.MapPost("/{agendamentoId:guid}/cancelamento", CancelarAsync)
            .WithName("CancelarAgendamento")
            .WithSummary("Cancela um agendamento")
            .WithDescription("Marca um agendamento existente como cancelado (UC06).")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOrder(4);
    }

    private static async Task<IResult> CancelarAsync(
        Guid agendamentoId, CancelarAgendamentoHandler handler, CancellationToken cancellationToken)
    {
        var resultado = await handler.ExecutarAsync(new CancelarAgendamentoComando(agendamentoId), cancellationToken);
        return resultado.ParaHttpResultado();
    }
}
