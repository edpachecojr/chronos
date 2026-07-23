using Chronos.Agenda.Application.Compartilhado;

namespace Chronos.Agenda.Application.Tests.Compartilhado.Fakes;

public class FakeMembroOrganizacaoRepositorioTests
{
    private readonly FakeMembroOrganizacaoRepositorio _repositorio = new();

    [Fact]
    public async Task BuscarOrganizacaoIdDoUsuario_apos_adicionar_retorna_a_organizacao_vinculada()
    {
        var usuarioId = Guid.NewGuid();
        var organizacaoId = Guid.NewGuid();
        await _repositorio.AdicionarAsync(usuarioId, organizacaoId, PapelMembroOrganizacao.Proprietario, CancellationToken.None);

        var encontrada = await _repositorio.BuscarOrganizacaoIdDoUsuarioAsync(usuarioId, CancellationToken.None);

        Assert.Equal(organizacaoId, encontrada);
    }

    [Fact]
    public async Task BuscarOrganizacaoIdDoUsuario_sem_vinculo_retorna_nulo()
    {
        var encontrada = await _repositorio.BuscarOrganizacaoIdDoUsuarioAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(encontrada);
    }

    [Fact]
    public async Task BuscarPapelDoUsuario_apos_adicionar_retorna_o_papel_vinculado()
    {
        var usuarioId = Guid.NewGuid();
        await _repositorio.AdicionarAsync(usuarioId, Guid.NewGuid(), PapelMembroOrganizacao.Proprietario, CancellationToken.None);

        var papel = await _repositorio.BuscarPapelDoUsuarioAsync(usuarioId, CancellationToken.None);

        Assert.Equal(PapelMembroOrganizacao.Proprietario, papel);
    }

    [Fact]
    public async Task BuscarPapelDoUsuario_sem_vinculo_retorna_nulo()
    {
        var papel = await _repositorio.BuscarPapelDoUsuarioAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(papel);
    }
}
