using Chronos.Agenda.Api.Autenticacao;
using Chronos.Agenda.Domain.Compartilhado.Contratos;

namespace Chronos.Agenda.Api.Extensions;

/// <summary>Registra os serviços específicos da composição da Api: o contexto do usuário corrente, que depende do
/// pipeline HTTP (ver <see cref="ResolucaoContextoUsuarioMiddleware"/>).</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Registra <see cref="ContextoUsuarioAcessor"/> e expõe <see cref="IContextoUsuario"/> a partir do
    /// valor que o middleware de resolução define para a requisição corrente.</summary>
    /// <example><code>builder.Services.AdicionarContextoUsuario();</code></example>
    public static IServiceCollection AdicionarContextoUsuario(this IServiceCollection servicos)
    {
        servicos.AddScoped<ContextoUsuarioAcessor>();
        servicos.AddScoped<IContextoUsuario>(provedor => provedor.GetRequiredService<ContextoUsuarioAcessor>().ObterContexto());

        return servicos;
    }
}
