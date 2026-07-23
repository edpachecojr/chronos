using Chronos.Agenda.Api.Erros;
using Chronos.Agenda.Application.Agendamentos.CriarAgendamento;

namespace Chronos.Agenda.Api.Endpoints.Agendamentos;

/// <summary>Endpoint de criação de um agendamento (UC04).</summary>
public sealed class CriarAgendamentoEndpoint : IEndpoint
{
    public static void MapearEndpoint(IEndpointRouteBuilder rotas)
    {
        rotas.MapPost("/", CriarAsync)
            .WithName("CriarAgendamento")
            .WithSummary("Cria um agendamento")
            .WithDescription("Agenda um atendimento de uma pessoa com um profissional para um serviço, dentro da disponibilidade configurada (UC04).")
            .Produces<CriarAgendamentoResultado>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithOrder(1);
    }

    private static async Task<IResult> CriarAsync(
        CriarAgendamentoComando comando, CriarAgendamentoHandler handler, CancellationToken cancellationToken)
    {
        var resultado = await handler.ExecutarAsync(comando, cancellationToken);
        return resultado.ParaHttpResultado(valor => Results.Created($"/v1/agendamentos/{valor.AgendamentoId}", valor));
    }
}
