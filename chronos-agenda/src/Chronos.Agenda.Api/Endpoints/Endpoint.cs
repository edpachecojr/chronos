using Chronos.Agenda.Api.Endpoints.Agendamentos;
using Chronos.Agenda.Api.Endpoints.Autenticacao;
using Chronos.Agenda.Api.Endpoints.Disponibilidades;
using Chronos.Agenda.Api.Endpoints.Organizacoes;
using Chronos.Agenda.Api.Endpoints.Profissionais;
using Chronos.Agenda.Api.Endpoints.Servicos;

namespace Chronos.Agenda.Api.Endpoints;

/// <summary>Composição de todos os endpoints da Api em um único ponto de entrada, versionados por prefixo de rota
/// (ADR 0001). Substitui a descoberta via injeção de dependências: cada <see cref="IEndpoint"/> é referenciado
/// explicitamente aqui, o que torna o mapa de rotas da aplicação visível em um único lugar.</summary>
public static class Endpoint
{
    /// <example><code>app.MapearEndpoints();</code></example>
    public static void MapearEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("");

        endpoints.MapGroup("v1/organizacoes")
            .WithTags("Organizações")
            .RequireAuthorization()
            .MapEndpoint<OnboardOrganizacaoEndpoint>()
            .MapEndpoint<ConsultarOrganizacaoAtualEndpoint>()
            .MapEndpoint<ConfigurarPerfilOperacionalEndpoint>();

        endpoints.MapGroup("v1/servicos")
            .WithTags("Serviços")
            .RequireAuthorization()
            .MapEndpoint<CriarServicoEndpoint>()
            .MapEndpoint<AtualizarServicoEndpoint>()
            .MapEndpoint<ListarServicosEndpoint>();

        endpoints.MapGroup("v1/disponibilidades")
            .WithTags("Disponibilidades")
            .RequireAuthorization()
            .MapEndpoint<CriarDisponibilidadeEndpoint>()
            .MapEndpoint<AlterarDisponibilidadeEndpoint>()
            .MapEndpoint<RemoverDisponibilidadeEndpoint>()
            .MapEndpoint<ListarDisponibilidadesEndpoint>();

        endpoints.MapGroup("v1/agendamentos")
            .WithTags("Agendamentos")
            .RequireAuthorization()
            .MapEndpoint<CriarAgendamentoEndpoint>()
            .MapEndpoint<ReagendarAgendamentoEndpoint>()
            .MapEndpoint<ConfirmarAgendamentoEndpoint>()
            .MapEndpoint<CancelarAgendamentoEndpoint>();

        endpoints.MapGroup("v1/profissionais")
            .WithTags("Profissionais")
            .RequireAuthorization()
            .MapEndpoint<ListarProfissionaisEndpoint>();

        endpoints.MapGroup("v1/profissionais/{profissionalId:guid}/agenda")
            .WithTags("Agenda")
            .RequireAuthorization()
            .MapEndpoint<ConsultarAgendaDiariaEndpoint>()
            .MapEndpoint<ConsultarAgendaSemanalEndpoint>();

        endpoints.MapGroup("v1/autenticacao")
            .WithTags("Autenticação")
            .MapEndpoint<AutenticacaoEndpoint>();
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder rotas) where TEndpoint : IEndpoint
    {
        TEndpoint.MapearEndpoint(rotas);
        return rotas;
    }
}
