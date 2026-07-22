using Chronos.Agenda.Application.Organizacoes.CasosDeUso;
using Chronos.Agenda.Application.Tests.Compartilhado.Fakes;
using Chronos.Agenda.Application.Tests.Organizacoes.Fakes;
using Chronos.Agenda.Application.Tests.Profissionais.Fakes;

namespace Chronos.Agenda.Application.Tests.Organizacoes.CasosDeUso;

public class CriarOrganizacaoCasoDeUsoTests
{
    private readonly FakeOrganizacaoRepositorio _organizacaoRepositorio = new();
    private readonly FakeProfissionalRepositorio _profissionalRepositorio = new();
    private readonly FakeMembroOrganizacaoRepositorio _membroOrganizacaoRepositorio = new();
    private readonly FakeUnidadeDeTrabalho _unidadeDeTrabalho = new();
    private readonly CriarOrganizacaoCasoDeUso _casoDeUso;

    public CriarOrganizacaoCasoDeUsoTests()
    {
        _casoDeUso = new CriarOrganizacaoCasoDeUso(
            _organizacaoRepositorio,
            _profissionalRepositorio,
            _membroOrganizacaoRepositorio,
            _unidadeDeTrabalho,
            new FakeProvedorDataHora(DateTime.UtcNow));
    }

    [Fact]
    public async Task ExecutarAsync_com_dados_validos_cria_organizacao_profissional_e_vinculo()
    {
        var usuarioId = Guid.NewGuid();
        var comando = new CriarOrganizacaoComando(usuarioId, "Clínica Bem-Estar", "Dra. Ana Souza");

        var resultado = await _casoDeUso.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        var organizacaoPersistida = await _organizacaoRepositorio.BuscarPorIdAsync(resultado.Valor.OrganizacaoId, CancellationToken.None);
        var profissionalPersistido = await _profissionalRepositorio.BuscarPorIdAsync(
            resultado.Valor.OrganizacaoId, resultado.Valor.ProfissionalId, CancellationToken.None);
        var organizacaoDoUsuario = await _membroOrganizacaoRepositorio.BuscarOrganizacaoIdDoUsuarioAsync(usuarioId, CancellationToken.None);
        Assert.NotNull(organizacaoPersistida);
        Assert.NotNull(profissionalPersistido);
        Assert.Equal(resultado.Valor.OrganizacaoId, organizacaoDoUsuario);
        Assert.Equal(1, _unidadeDeTrabalho.VezesSalvo);
    }

    [Fact]
    public async Task ExecutarAsync_com_nome_de_organizacao_invalido_retorna_falha_e_nao_persiste_nada()
    {
        var comando = new CriarOrganizacaoComando(Guid.NewGuid(), "", "Dra. Ana Souza");

        var resultado = await _casoDeUso.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Organizacao.NomeInvalido", resultado.Erro!.Codigo);
        Assert.Equal(0, _unidadeDeTrabalho.VezesSalvo);
    }

    [Fact]
    public async Task ExecutarAsync_com_nome_de_profissional_invalido_retorna_falha_e_nao_persiste_a_organizacao()
    {
        var comando = new CriarOrganizacaoComando(Guid.NewGuid(), "Clínica Bem-Estar", "");

        var resultado = await _casoDeUso.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Profissional.NomeInvalido", resultado.Erro!.Codigo);
        Assert.Equal(0, _unidadeDeTrabalho.VezesSalvo);
    }
}
