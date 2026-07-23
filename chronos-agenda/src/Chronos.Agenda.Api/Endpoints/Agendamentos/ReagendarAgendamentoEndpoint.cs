using Chronos.Agenda.Api.Erros;
using Chronos.Agenda.Application.Agendamentos.ReagendarAgendamento;

namespace Chronos.Agenda.Api.Endpoints.Agendamentos;

/// <summary>Endpoint de reagendamento de um agendamento existente (UC05).</summary>
public sealed class ReagendarAgendamentoEndpoint : IEndpoint
{
    public static void MapearEndpoint(IEndpointRouteBuilder rotas)
    {
        rotas.MapPut("/{agendamentoId:guid}", ReagendarAsync)
            .WithName("ReagendarAgendamento")
            .WithSummary("Reagenda um agendamento")
            .WithDescription("Altera a pessoa atendida e o período de um agendamento existente, sem trocar profissional ou serviço (UC05).")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithOrder(2);
    }

    private static async Task<IResult> ReagendarAsync(
        Guid agendamentoId, ReagendarAgendamentoRequest requisicao, ReagendarAgendamentoHandler handler, CancellationToken cancellationToken)
    {
        var comando = new ReagendarAgendamentoComando(
            agendamentoId, requisicao.ProfissionalId, requisicao.ServicoId, requisicao.NomePessoaAtendida,
            requisicao.TipoPessoaAtendida, requisicao.Inicio, requisicao.EnderecoPessoaAtendida);
        var resultado = await handler.ExecutarAsync(comando, cancellationToken);
        return resultado.ParaHttpResultado();
    }
}
