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
}
