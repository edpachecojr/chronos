using Chronos.Agenda.Domain.Compartilhado.Contratos;

namespace Chronos.Agenda.Api.Autenticacao;

/// <summary>Guarda o <see cref="IContextoUsuario"/> resolvido uma única vez por requisição por
/// <see cref="ResolucaoContextoUsuarioMiddleware"/>. Existe porque <see cref="IContextoUsuario.ObterOrganizacaoId"/>
/// é síncrono no contrato de domínio, enquanto a consulta ao vínculo usuário↔organização é assíncrona (ver Fase 0
/// do plano de implementação do MVP).</summary>
public sealed class ContextoUsuarioAcessor
{
    private IContextoUsuario? contexto;

    public void Definir(IContextoUsuario contextoResolvido)
    {
        contexto = contextoResolvido;
    }

    /// <summary>Obtém o contexto já resolvido para a requisição corrente.</summary>
    /// <example><code>var organizacaoId = acessor.ObterContexto().ObterOrganizacaoId();</code></example>
    public IContextoUsuario ObterContexto()
    {
        return contexto ?? throw new InvalidOperationException(
            "O contexto do usuário ainda não foi resolvido para esta requisição. Verifique se " +
            $"{nameof(ResolucaoContextoUsuarioMiddleware)} está registrado no pipeline antes do uso de {nameof(IContextoUsuario)}.");
    }
}
