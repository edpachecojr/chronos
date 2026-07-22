using Chronos.Agenda.Application.Compartilhado;
using Chronos.Agenda.Application.Servicos.CriarServico;
using Chronos.Agenda.Application.Tests.Compartilhado.Fakes;
using Chronos.Agenda.Application.Tests.Profissionais.Fakes;
using Chronos.Agenda.Application.Tests.Servicos.Fakes;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Profissionais.Entidades;
using Chronos.Agenda.Domain.Servicos.Enums;

namespace Chronos.Agenda.Application.Tests.Servicos.CriarServico;

public class CriarServicoHandlerTests
{
    private static readonly DateTime CriadoEmUtc = new(2026, 7, 22, 12, 0, 0, DateTimeKind.Utc);

    private readonly Guid _organizacaoId = Guid.NewGuid();
    private readonly FakeServicoRepositorio _servicoRepositorio = new();
    private readonly FakeProfissionalRepositorio _profissionalRepositorio = new();
    private readonly FakeUnidadeDeTrabalho _unidadeDeTrabalho = new();
    private readonly CriarServicoHandler _handler;
    private readonly Profissional _profissional;

    public CriarServicoHandlerTests()
    {
        var provedorDataHora = new FakeProvedorDataHora(CriadoEmUtc);
        _profissional = Profissional.Criar(_organizacaoId, new Nome("Dra. Ana Souza"), provedorDataHora);
        _profissionalRepositorio.AdicionarAsync(_profissional, CancellationToken.None).GetAwaiter().GetResult();
        _handler = new CriarServicoHandler(
            _servicoRepositorio,
            _profissionalRepositorio,
            new ContextoUsuario(Guid.NewGuid(), _organizacaoId),
            _unidadeDeTrabalho,
            provedorDataHora);
    }

    [Fact]
    public async Task ExecutarAsync_com_dados_validos_cria_servico()
    {
        var comando = new CriarServicoComando(_profissional.Id, "Consulta inicial", TimeSpan.FromMinutes(50), 250m, TipoAtendimento.Online);

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        var persistido = await _servicoRepositorio.BuscarPorIdAsync(_organizacaoId, resultado.Valor.ServicoId, CancellationToken.None);
        Assert.NotNull(persistido);
        Assert.Equal("Consulta inicial", persistido.Nome.Valor);
        Assert.Equal(1, _unidadeDeTrabalho.VezesSalvo);
    }

    [Fact]
    public async Task ExecutarAsync_com_profissional_inexistente_retorna_falha()
    {
        var comando = new CriarServicoComando(Guid.NewGuid(), "Consulta inicial", TimeSpan.FromMinutes(50), 250m, TipoAtendimento.Online);

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Profissional.NaoEncontrado", resultado.Erro!.Codigo);
        Assert.Equal(0, _unidadeDeTrabalho.VezesSalvo);
    }

    [Theory]
    [InlineData("", 50, 250, "Servico.NomeInvalido")]
    public async Task ExecutarAsync_com_nome_invalido_retorna_falha(string nome, int duracaoEmMinutos, decimal preco, string codigoEsperado)
    {
        var comando = new CriarServicoComando(_profissional.Id, nome, TimeSpan.FromMinutes(duracaoEmMinutos), preco, TipoAtendimento.Online);

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal(codigoEsperado, resultado.Erro!.Codigo);
    }

    [Fact]
    public async Task ExecutarAsync_com_duracao_invalida_retorna_falha()
    {
        var comando = new CriarServicoComando(_profissional.Id, "Consulta inicial", TimeSpan.Zero, 250m, TipoAtendimento.Online);

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Servico.DuracaoInvalida", resultado.Erro!.Codigo);
    }

    [Fact]
    public async Task ExecutarAsync_com_preco_invalido_retorna_falha()
    {
        var comando = new CriarServicoComando(_profissional.Id, "Consulta inicial", TimeSpan.FromMinutes(50), -1m, TipoAtendimento.Online);

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Servico.PrecoInvalido", resultado.Erro!.Codigo);
    }
}
