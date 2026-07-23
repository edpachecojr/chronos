using Chronos.Agenda.Infrastructure.Identity;

namespace Chronos.Agenda.Api.Endpoints.Autenticacao;

/// <summary>Expõe o registro, login e demais operações de conta do ASP.NET Core Identity (ADR 0006), usando tokens
/// de portador nativos como mecanismo de sessão. Não usa <see cref="Application.Compartilhado.Contratos.IServicoAutenticacao"/>
/// porque <c>MapIdentityApi</c> já orquestra <c>UserManager</c>/<c>SignInManager</c> e a emissão do token — esse
/// contrato de Aplicação existe para os casos de uso que precisarem de autenticação sem depender do Identity
/// diretamente, o que ainda não é o caso.
///
/// <c>MapIdentityApi</c> mapeia várias rotas internamente (registro, login, refresh, confirmação de e-mail, etc.);
/// por isso este endpoint não define nome, resumo, descrição ou "produces" individuais como os demais — atribuir os
/// mesmos valores a todas as rotas do bundle seria impreciso e um nome repetido causaria conflito de nomes de
/// endpoint no ASP.NET Core.</summary>
public sealed class AutenticacaoEndpoint : IEndpoint
{
    public static void MapearEndpoint(IEndpointRouteBuilder rotas)
    {
        rotas.MapIdentityApi<UsuarioIdentity>();
    }
}
