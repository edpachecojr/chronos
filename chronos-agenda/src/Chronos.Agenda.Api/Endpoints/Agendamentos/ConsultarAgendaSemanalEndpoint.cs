using Chronos.Agenda.Api.Erros;
using Chronos.Agenda.Application.Agendamentos.ConsultarAgendaSemanal;

namespace Chronos.Agenda.Api.Endpoints.Agendamentos;

/// <summary>Endpoint de consulta da agenda de um profissional ao longo de uma semana local (UC07).</summary>
public sealed class ConsultarAgendaSemanalEndpoint : IEndpoint
{
    public static void MapearEndpoint(IEndpointRouteBuilder rotas)
    {
        rotas.MapGet("/semanal", ConsultarAsync)
            .WithName("ConsultarAgendaSemanal")
            .WithSummary("Consulta a agenda semanal")
            .WithDescription("Projeta as janelas de disponibilidade e os agendamentos ativos de um profissional ao longo dos sete dias a partir do início da semana informado (UC07).")
            .Produces<AgendaSemanalResultado>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOrder(2);
    }

    private static async Task<IResult> ConsultarAsync(
        Guid profissionalId, DateOnly inicioDaSemana, ConsultarAgendaSemanalHandler handler, CancellationToken cancellationToken)
    {
        var resultado = await handler.ExecutarAsync(new ConsultarAgendaSemanalConsulta(profissionalId, inicioDaSemana), cancellationToken);
        return resultado.ParaHttpResultado(Results.Ok);
    }
}
