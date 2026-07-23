using Chronos.Agenda.Api.Erros;
using Chronos.Agenda.Application.Agendamentos.ConfirmarAgendamento;

namespace Chronos.Agenda.Api.Endpoints.Agendamentos;

/// <summary>Endpoint de confirmação de um agendamento existente (UC06).</summary>
public sealed class ConfirmarAgendamentoEndpoint : IEndpoint
{
    public static void MapearEndpoint(IEndpointRouteBuilder rotas)
    {
        rotas.MapPost("/{agendamentoId:guid}/confirmacao", ConfirmarAsync)
            .WithName("ConfirmarAgendamento")
            .WithSummary("Confirma um agendamento")
            .WithDescription("Marca um agendamento existente como confirmado (UC06).")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOrder(3);
    }

    private static async Task<IResult> ConfirmarAsync(
        Guid agendamentoId, ConfirmarAgendamentoHandler handler, CancellationToken cancellationToken)
    {
        var resultado = await handler.ExecutarAsync(new ConfirmarAgendamentoComando(agendamentoId), cancellationToken);
        return resultado.ParaHttpResultado();
    }
}
