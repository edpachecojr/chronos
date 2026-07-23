using Chronos.Agenda.Api.Autenticacao;

namespace Chronos.Agenda.Api.Extensions;

/// <summary>Registra o middleware que resolve o contexto do usuário corrente no pipeline HTTP.</summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>Adiciona <see cref="ResolucaoContextoUsuarioMiddleware"/> ao pipeline. Deve vir depois de
    /// <c>UseAuthentication</c>, pois depende de <c>HttpContext.User</c> já estar preenchido.</summary>
    /// <example><code>app.UseAuthentication();
    /// app.UsarContextoUsuario();
    /// app.UseAuthorization();</code></example>
    public static IApplicationBuilder UsarContextoUsuario(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ResolucaoContextoUsuarioMiddleware>();
    }

    /// <summary>Habilita a documentação interativa do Swagger em desenvolvimento. A interface fica acessível em
    /// <c>GET /swagger/index.html</c> e o JSON da especificação OpenAPI em <c>GET /swagger/v1/swagger.json</c>.</summary>
    /// <example><code>if (app.Environment.IsDevelopment())
    ///     app.UsarSwagger();</code></example>
    public static IApplicationBuilder UsarSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(opcoes =>
        {
            opcoes.RoutePrefix = "swagger";
            opcoes.SwaggerEndpoint("/swagger/v1/swagger.json", "Chronos Agenda API v1");
        });

        return app;
    }
}
