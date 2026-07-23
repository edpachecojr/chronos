using Chronos.Agenda.Application.Tests.Compartilhado.Fakes;
using Chronos.Agenda.Domain.Organizacoes.Entidades;
using Chronos.Agenda.Domain.Organizacoes.ObjetosValor;

namespace Chronos.Agenda.Application.Tests.Organizacoes.Fakes;

public class FakeOrganizacaoRepositorioTests
{
    private readonly FakeOrganizacaoRepositorio _repositorio = new();
    private readonly FakeProvedorDataHora _provedorDataHora = new(DateTime.UtcNow);

    [Fact]
    public async Task BuscarPorId_apos_adicionar_retorna_a_organizacao_persistida()
    {
        var organizacao = Organizacao.Criar(new NomeOrganizacao("Clínica Exemplo"), _provedorDataHora);
        await _repositorio.AdicionarAsync(organizacao, CancellationToken.None);

        var encontrada = await _repositorio.BuscarPorIdAsync(organizacao.Id, CancellationToken.None);

        Assert.Same(organizacao, encontrada);
    }

    [Fact]
    public async Task BuscarPorId_com_identificador_inexistente_retorna_nulo()
    {
        var encontrada = await _repositorio.BuscarPorIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(encontrada);
    }

    [Fact]
    public async Task AtualizarAsync_substitui_a_organizacao_persistida_pelo_mesmo_identificador()
    {
        var organizacao = Organizacao.Criar(new NomeOrganizacao("Clínica Exemplo"), _provedorDataHora);
        await _repositorio.AdicionarAsync(organizacao, CancellationToken.None);
        organizacao.ConfigurarPerfilOperacional(null, new FusoHorario("America/Sao_Paulo"), _provedorDataHora);

        await _repositorio.AtualizarAsync(organizacao, CancellationToken.None);

        var encontrada = await _repositorio.BuscarPorIdAsync(organizacao.Id, CancellationToken.None);
        Assert.Equal("America/Sao_Paulo", encontrada!.FusoHorario!.Identificador);
    }
}
