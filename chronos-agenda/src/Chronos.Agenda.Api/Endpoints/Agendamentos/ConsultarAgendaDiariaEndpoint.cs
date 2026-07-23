using Chronos.Agenda.Api.Erros;
using Chronos.Agenda.Application.Agendamentos;
using Chronos.Agenda.Application.Agendamentos.ConsultarAgendaDiaria;

namespace Chronos.Agenda.Api.Endpoints.Agendamentos;

/// <summary>Endpoint de consulta da agenda de um profissional em um único dia local (UC07).</summary>
public sealed class ConsultarAgendaDiariaEndpoint : IEndpoint
{
    public static void MapearEndpoint(IEndpointRouteBuilder rotas)
    {
        rotas.MapGet("/diaria", ConsultarAsync)
            .WithName("ConsultarAgendaDiaria")
            .WithSummary("Consulta a agenda diária")
            .WithDescription("Projeta as janelas de disponibilidade e os agendamentos ativos de um profissional em um único dia local (UC07).")
            .Produces<AgendaDiariaResultado>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOrder(1);
    }

    private static async Task<IResult> ConsultarAsync(
        Guid profissionalId, DateOnly data, ConsultarAgendaDiariaHandler handler, CancellationToken cancellationToken)
    {
        var resultado = await handler.ExecutarAsync(new ConsultarAgendaDiariaConsulta(profissionalId, data), cancellationToken);
        return resultado.ParaHttpResultado(Results.Ok);
    }
}
