using Chronos.Agenda.Api.Erros;
using Chronos.Agenda.Application.Servicos.CriarServico;

namespace Chronos.Agenda.Api.Endpoints.Servicos;

/// <summary>Endpoint de criação de um serviço no catálogo de um profissional (UC03).</summary>
public sealed class CriarServicoEndpoint : IEndpoint
{
    public static void MapearEndpoint(IEndpointRouteBuilder rotas)
    {
        rotas.MapPost("/", CriarAsync)
            .WithName("CriarServico")
            .WithSummary("Cria um serviço")
            .WithDescription("Adiciona um serviço ao catálogo de um profissional, com sua configuração comercial (duração, preço e tipo de atendimento) (UC03).")
            .Produces<CriarServicoResultado>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOrder(1);
    }

    private static async Task<IResult> CriarAsync(
        CriarServicoComando comando, CriarServicoHandler handler, CancellationToken cancellationToken)
    {
        var resultado = await handler.ExecutarAsync(comando, cancellationToken);
        return resultado.ParaHttpResultado(valor => Results.Created($"/v1/servicos/{valor.ServicoId}", valor));
    }
}
