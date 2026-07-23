using Chronos.Agenda.Api.Erros;
using Chronos.Agenda.Application.Organizacoes.CriarOrganizacao;
using Chronos.Agenda.Domain.Compartilhado.Contratos;

namespace Chronos.Agenda.Api.Endpoints.Organizacoes;

/// <summary>Endpoint de onboarding de uma organização (UC01).</summary>
public sealed class OnboardOrganizacaoEndpoint : IEndpoint
{
    public static void MapearEndpoint(IEndpointRouteBuilder rotas)
    {
        rotas.MapPost("/", OnboardAsync)
            .WithName("OnboardOrganizacao")
            .WithSummary("Realiza o onboarding de uma organização")
            .WithDescription("Cria a organização e o profissional inicial vinculado ao usuário autenticado, iniciando o ciclo de vida do tenant (UC01).")
            .Produces<CriarOrganizacaoResultado>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOrder(1);
    }

    private static async Task<IResult> OnboardAsync(
        OnboardOrganizacaoRequest requisicao,
        CriarOrganizacaoHandler handler,
        IContextoUsuario contextoUsuario,
        CancellationToken cancellationToken)
    {
        var comando = new CriarOrganizacaoComando(contextoUsuario.UsuarioId, requisicao.Nome, requisicao.NomeProfissionalInicial);
        var resultado = await handler.ExecutarAsync(comando, cancellationToken);
        return resultado.ParaHttpResultado(valor => Results.Created($"/v1/organizacoes/{valor.OrganizacaoId}", valor));
    }
}
