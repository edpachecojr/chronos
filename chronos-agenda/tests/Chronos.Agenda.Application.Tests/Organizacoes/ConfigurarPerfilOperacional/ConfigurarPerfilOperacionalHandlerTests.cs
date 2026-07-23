using Chronos.Agenda.Application.Compartilhado;
using Chronos.Agenda.Application.Organizacoes.ConfigurarPerfilOperacional;
using Chronos.Agenda.Application.Tests.Compartilhado.Fakes;
using Chronos.Agenda.Application.Tests.Organizacoes.Fakes;
using Chronos.Agenda.Domain.Organizacoes.Entidades;
using Chronos.Agenda.Domain.Organizacoes.ObjetosValor;

namespace Chronos.Agenda.Application.Tests.Organizacoes.ConfigurarPerfilOperacional;

public class ConfigurarPerfilOperacionalHandlerTests
{
    private static readonly DateTime CriadoEmUtc = new(2026, 7, 23, 12, 0, 0, DateTimeKind.Utc);

    private readonly Guid _organizacaoId;
    private readonly FakeOrganizacaoRepositorio _organizacaoRepositorio = new();
    private readonly FakeUnidadeDeTrabalho _unidadeDeTrabalho = new();
    private readonly ConfigurarPerfilOperacionalHandler _handler;

    public ConfigurarPerfilOperacionalHandlerTests()
    {
        var provedorDataHora = new FakeProvedorDataHora(CriadoEmUtc);
        var organizacao = Organizacao.Criar(new NomeOrganizacao("Clínica Exemplo"), provedorDataHora);
        _organizacaoId = organizacao.Id;
        _organizacaoRepositorio.AdicionarAsync(organizacao, CancellationToken.None).GetAwaiter().GetResult();
        _handler = new ConfigurarPerfilOperacionalHandler(
            _organizacaoRepositorio, new ContextoUsuario(Guid.NewGuid(), _organizacaoId), _unidadeDeTrabalho, provedorDataHora);
    }

    [Fact]
    public async Task ExecutarAsync_com_dados_validos_configura_endereco_e_fuso_horario()
    {
        var comando = new ConfigurarPerfilOperacionalComando("Av. Central, 20", "America/Sao_Paulo");

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        var organizacao = await _organizacaoRepositorio.BuscarPorIdAsync(_organizacaoId, CancellationToken.None);
        Assert.Equal("Av. Central, 20", organizacao!.EnderecoPrestador!.Descricao);
        Assert.Equal("America/Sao_Paulo", organizacao.FusoHorario!.Identificador);
        Assert.Equal(1, _unidadeDeTrabalho.VezesSalvo);
    }

    [Fact]
    public async Task ExecutarAsync_sem_endereco_permite_organizacao_somente_online()
    {
        var comando = new ConfigurarPerfilOperacionalComando(null, "America/Sao_Paulo");

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        var organizacao = await _organizacaoRepositorio.BuscarPorIdAsync(_organizacaoId, CancellationToken.None);
        Assert.Null(organizacao!.EnderecoPrestador);
        Assert.Equal("America/Sao_Paulo", organizacao.FusoHorario!.Identificador);
    }

    [Fact]
    public async Task ExecutarAsync_com_fuso_horario_invalido_retorna_falha_e_nao_persiste()
    {
        var comando = new ConfigurarPerfilOperacionalComando(null, "Fuso/Inexistente");

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Organizacao.FusoHorarioInvalido", resultado.Erro!.Codigo);
        Assert.Equal(0, _unidadeDeTrabalho.VezesSalvo);
    }

    [Fact]
    public async Task ExecutarAsync_com_endereco_invalido_retorna_falha_e_nao_persiste()
    {
        var comando = new ConfigurarPerfilOperacionalComando(new string('a', 301), "America/Sao_Paulo");

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Organizacao.EnderecoInvalido", resultado.Erro!.Codigo);
        Assert.Equal(0, _unidadeDeTrabalho.VezesSalvo);
    }

    [Fact]
    public async Task ExecutarAsync_com_organizacao_inexistente_retorna_falha()
    {
        var contextoDeOutraOrganizacao = new ContextoUsuario(Guid.NewGuid(), Guid.NewGuid());
        var handler = new ConfigurarPerfilOperacionalHandler(
            _organizacaoRepositorio, contextoDeOutraOrganizacao, _unidadeDeTrabalho, new FakeProvedorDataHora(CriadoEmUtc));
        var comando = new ConfigurarPerfilOperacionalComando(null, "America/Sao_Paulo");

        var resultado = await handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Organizacao.NaoEncontrada", resultado.Erro!.Codigo);
        Assert.Equal(0, _unidadeDeTrabalho.VezesSalvo);
    }
}
