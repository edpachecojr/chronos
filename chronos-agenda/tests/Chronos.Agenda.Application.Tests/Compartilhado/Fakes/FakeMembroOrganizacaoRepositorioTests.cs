namespace Chronos.Agenda.Application.Tests.Compartilhado.Fakes;

public class FakeMembroOrganizacaoRepositorioTests
{
    private readonly FakeMembroOrganizacaoRepositorio _repositorio = new();

    [Fact]
    public async Task BuscarOrganizacaoIdDoUsuario_apos_adicionar_retorna_a_organizacao_vinculada()
    {
        var usuarioId = Guid.NewGuid();
        var organizacaoId = Guid.NewGuid();
        await _repositorio.AdicionarAsync(usuarioId, organizacaoId, CancellationToken.None);

        var encontrada = await _repositorio.BuscarOrganizacaoIdDoUsuarioAsync(usuarioId, CancellationToken.None);

        Assert.Equal(organizacaoId, encontrada);
    }

    [Fact]
    public async Task BuscarOrganizacaoIdDoUsuario_sem_vinculo_retorna_nulo()
    {
        var encontrada = await _repositorio.BuscarOrganizacaoIdDoUsuarioAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(encontrada);
    }
}
