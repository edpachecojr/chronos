using Chronos.Agenda.Api.Erros;
using Chronos.Agenda.Application.Disponibilidades.CriarDisponibilidade;

namespace Chronos.Agenda.Api.Endpoints.Disponibilidades;

/// <summary>Endpoint de criação de uma janela de disponibilidade semanal de um profissional (UC02).</summary>
public sealed class CriarDisponibilidadeEndpoint : IEndpoint
{
    public static void MapearEndpoint(IEndpointRouteBuilder rotas)
    {
        rotas.MapPost("/", CriarAsync)
            .WithName("CriarDisponibilidade")
            .WithSummary("Cria uma disponibilidade")
            .WithDescription("Adiciona uma janela de disponibilidade semanal (dia da semana, início e fim) para um profissional (UC02).")
            .Produces<CriarDisponibilidadeResultado>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithOrder(1);
    }

    private static async Task<IResult> CriarAsync(
        CriarDisponibilidadeComando comando, CriarDisponibilidadeHandler handler, CancellationToken cancellationToken)
    {
        var resultado = await handler.ExecutarAsync(comando, cancellationToken);
        return resultado.ParaHttpResultado(valor => Results.Created($"/v1/disponibilidades/{valor.DisponibilidadeId}", valor));
    }
}
