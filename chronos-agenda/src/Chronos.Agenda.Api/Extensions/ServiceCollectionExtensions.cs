using Chronos.Agenda.Api.Autenticacao;
using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Microsoft.OpenApi.Models;

namespace Chronos.Agenda.Api.Extensions;

/// <summary>Registra os serviços específicos da composição da Api: o contexto do usuário corrente, que depende do
/// pipeline HTTP (ver <see cref="ResolucaoContextoUsuarioMiddleware"/>), a política de CORS que autoriza o
/// frontend a consumir a Api a partir de uma origem diferente (ADR 0007: SPA Vite consumindo a Api via REST), e
/// a geração automática de documentação OpenAPI via Swagger em desenvolvimento.
/// </summary>
public static class ServiceCollectionExtensions
{
    public const string PoliticaCorsFrontend = "FrontendChronosApp";

    /// <summary>Registra <see cref="ContextoUsuarioAcessor"/> e expõe <see cref="IContextoUsuario"/> a partir do
    /// valor que o middleware de resolução define para a requisição corrente.</summary>
    /// <example><code>builder.Services.AdicionarContextoUsuario();</code></example>
    public static IServiceCollection AdicionarContextoUsuario(this IServiceCollection servicos)
    {
        servicos.AddScoped<ContextoUsuarioAcessor>();
        servicos.AddScoped<IContextoUsuario>(provedor => provedor.GetRequiredService<ContextoUsuarioAcessor>().ObterContexto());

        return servicos;
    }

    /// <summary>Autoriza, via CORS, as origens do frontend lidas de <c>Cors:OrigensPermitidas</c>. As credenciais
    /// (cabeçalho <c>Authorization</c>) são exigidas pelo bearer token nativo do Identity (ADR 0006), por isso a
    /// política não pode usar um curinga de origem.</summary>
    /// <example><code>builder.Services.AdicionarCorsFrontend(builder.Configuration);</code></example>
    public static IServiceCollection AdicionarCorsFrontend(this IServiceCollection servicos, IConfiguration configuracao)
    {
        var origensPermitidas = configuracao.GetSection("Cors:OrigensPermitidas").Get<string[]>() ?? [];

        servicos.AddCors(opcoes => opcoes.AddPolicy(PoliticaCorsFrontend, politica =>
            politica
                .WithOrigins(origensPermitidas)
                .AllowAnyHeader()
                .AllowAnyMethod()));

        return servicos;
    }

    /// <summary>Registra o gerador de documentação OpenAPI (Swagger) para ambiente de desenvolvimento. A interface
    /// interativa está disponível em <c>/swagger/index.html</c>.</summary>
    /// <example><code>builder.Services.AdicionarSwagger();</code></example>
    public static IServiceCollection AdicionarSwagger(this IServiceCollection servicos)
    {
        servicos.AddEndpointsApiExplorer();
        servicos.AddSwaggerGen(opcoes =>
        {
            opcoes.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Chronos Agenda API",
                Version = "v1",
                Description = "API de agendamentos do Chronos",
                Contact = new OpenApiContact
                {
                    Name = "Chronos",
                    Email = "contato@chronos.local"
                }
            });

            opcoes.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Bearer token JWT para autenticação"
            });

            opcoes.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return servicos;
    }
}
