using Chronos.Agenda.Api.Erros;
using Chronos.Agenda.Application.Disponibilidades.AlterarDisponibilidade;

namespace Chronos.Agenda.Api.Endpoints.Disponibilidades;

/// <summary>Endpoint de alteração de uma janela de disponibilidade semanal existente (UC02).</summary>
public sealed class AlterarDisponibilidadeEndpoint : IEndpoint
{
    public static void MapearEndpoint(IEndpointRouteBuilder rotas)
    {
        rotas.MapPut("/{disponibilidadeId:guid}", AlterarAsync)
            .WithName("AlterarDisponibilidade")
            .WithSummary("Altera uma disponibilidade")
            .WithDescription("Altera o dia da semana e a janela de início/fim de uma disponibilidade existente do profissional (UC02).")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithOrder(2);
    }

    private static async Task<IResult> AlterarAsync(
        Guid disponibilidadeId, AlterarDisponibilidadeRequest requisicao, AlterarDisponibilidadeHandler handler, CancellationToken cancellationToken)
    {
        var comando = new AlterarDisponibilidadeComando(
            requisicao.ProfissionalId, disponibilidadeId, requisicao.DiaDaSemana, requisicao.Inicio, requisicao.Fim);
        var resultado = await handler.ExecutarAsync(comando, cancellationToken);
        return resultado.ParaHttpResultado();
    }
}
