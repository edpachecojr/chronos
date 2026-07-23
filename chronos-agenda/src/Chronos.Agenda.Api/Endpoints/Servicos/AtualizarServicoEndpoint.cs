using Chronos.Agenda.Api.Erros;
using Chronos.Agenda.Application.Servicos.AtualizarServico;

namespace Chronos.Agenda.Api.Endpoints.Servicos;

/// <summary>Endpoint de atualização da configuração comercial de um serviço existente (UC03).</summary>
public sealed class AtualizarServicoEndpoint : IEndpoint
{
    public static void MapearEndpoint(IEndpointRouteBuilder rotas)
    {
        rotas.MapPut("/{servicoId:guid}", AtualizarAsync)
            .WithName("AtualizarServico")
            .WithSummary("Atualiza um serviço")
            .WithDescription("Atualiza nome, duração, preço e tipo de atendimento de um serviço existente do profissional (UC03).")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOrder(2);
    }

    private static async Task<IResult> AtualizarAsync(
        Guid servicoId, AtualizarServicoRequest requisicao, AtualizarServicoHandler handler, CancellationToken cancellationToken)
    {
        var comando = new AtualizarServicoComando(servicoId, requisicao.Nome, requisicao.Duracao, requisicao.Preco, requisicao.TipoAtendimento);
        var resultado = await handler.ExecutarAsync(comando, cancellationToken);
        return resultado.ParaHttpResultado();
    }
}
