using Chronos.Agenda.Application.Compartilhado;

namespace Chronos.Agenda.Application.Tests.Compartilhado;

public class ContextoUsuarioTests
{
    [Fact]
    public void ObterOrganizacaoId_retorna_a_organizacao_resolvida_na_construcao()
    {
        var usuarioId = Guid.NewGuid();
        var organizacaoId = Guid.NewGuid();
        var contexto = new ContextoUsuario(usuarioId, organizacaoId);

        Assert.Equal(usuarioId, contexto.UsuarioId);
        Assert.Equal(organizacaoId, contexto.ObterOrganizacaoId());
    }
}
