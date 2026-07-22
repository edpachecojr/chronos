using System.Security.Claims;
using Chronos.Agenda.Application.Compartilhado;
using Chronos.Agenda.Application.Compartilhado.Contratos;

namespace Chronos.Agenda.Api.Autenticacao;

/// <summary>Resolve, uma única vez por requisição autenticada, o usuário corrente e a organização à qual pertence
/// (RN01, ADR 0003), registrando o resultado em <see cref="ContextoUsuarioAcessor"/> para que
/// <see cref="IContextoUsuario"/> possa ser consumido de forma síncrona pelos casos de uso.</summary>
public sealed class ResolucaoContextoUsuarioMiddleware(RequestDelegate proximo)
{
    public async Task InvokeAsync(HttpContext httpContext, ContextoUsuarioAcessor acessor, IMembroOrganizacaoRepositorio membros)
    {
        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            var usuarioId = ExtrairUsuarioId(httpContext.User);
            var organizacaoId = await membros.BuscarOrganizacaoIdDoUsuarioAsync(usuarioId, httpContext.RequestAborted);
            acessor.Definir(new ContextoUsuario(usuarioId, organizacaoId ?? Guid.Empty));
        }

        await proximo(httpContext);
    }

    private static Guid ExtrairUsuarioId(ClaimsPrincipal usuario)
    {
        var valorReivindicado = usuario.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(valorReivindicado, out var usuarioId)
            ? usuarioId
            : throw new InvalidOperationException(
                $"O identificador do usuário autenticado ('{valorReivindicado}') não é um GUID válido.");
    }
}
