using Chronos.Agenda.Application.Compartilhado;
using Chronos.Agenda.Application.Servicos.AtualizarServico;
using Chronos.Agenda.Application.Tests.Compartilhado.Fakes;
using Chronos.Agenda.Application.Tests.Servicos.Fakes;
using Chronos.Agenda.Domain.Servicos.Entidades;
using Chronos.Agenda.Domain.Servicos.Enums;
using Chronos.Agenda.Domain.Servicos.ObjetosValor;

namespace Chronos.Agenda.Application.Tests.Servicos.AtualizarServico;

public class AtualizarServicoHandlerTests
{
    private static readonly DateTime CriadoEmUtc = new(2026, 7, 22, 12, 0, 0, DateTimeKind.Utc);

    private readonly Guid _organizacaoId = Guid.NewGuid();
    private readonly FakeServicoRepositorio _servicoRepositorio = new();
    private readonly FakeUnidadeDeTrabalho _unidadeDeTrabalho = new();
    private readonly AtualizarServicoHandler _handler;
    private readonly Servico _servico;

    public AtualizarServicoHandlerTests()
    {
        var provedorDataHora = new FakeProvedorDataHora(CriadoEmUtc);
        _servico = Servico.Criar(
            _organizacaoId, Guid.NewGuid(), new NomeServico("Consulta inicial"), new DuracaoServico(TimeSpan.FromMinutes(50)),
            new PrecoServico(250m), TipoAtendimento.Online, provedorDataHora);
        _servicoRepositorio.AdicionarAsync(_servico, CancellationToken.None).GetAwaiter().GetResult();
        _handler = new AtualizarServicoHandler(
            _servicoRepositorio, new ContextoUsuario(Guid.NewGuid(), _organizacaoId), _unidadeDeTrabalho, provedorDataHora);
    }

    [Fact]
    public async Task ExecutarAsync_com_dados_validos_atualiza_a_configuracao_comercial()
    {
        var comando = new AtualizarServicoComando(_servico.Id, "Consulta de retorno", TimeSpan.FromMinutes(30), 180m, TipoAtendimento.Domiciliar);

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.Equal("Consulta de retorno", _servico.Nome.Valor);
        Assert.Equal(TimeSpan.FromMinutes(30), _servico.Duracao.Valor);
        Assert.Equal(180m, _servico.Preco.Valor);
        Assert.Equal(TipoAtendimento.Domiciliar, _servico.TipoAtendimento);
        Assert.Equal(1, _unidadeDeTrabalho.VezesSalvo);
    }

    [Fact]
    public async Task ExecutarAsync_com_servico_inexistente_retorna_falha()
    {
        var comando = new AtualizarServicoComando(Guid.NewGuid(), "Consulta de retorno", TimeSpan.FromMinutes(30), 180m, TipoAtendimento.Online);

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Servico.NaoEncontrado", resultado.Erro!.Codigo);
        Assert.Equal(0, _unidadeDeTrabalho.VezesSalvo);
    }

    [Fact]
    public async Task ExecutarAsync_com_nome_invalido_retorna_falha_e_nao_altera_o_servico()
    {
        var comando = new AtualizarServicoComando(_servico.Id, "", TimeSpan.FromMinutes(30), 180m, TipoAtendimento.Online);

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Servico.NomeInvalido", resultado.Erro!.Codigo);
        Assert.Equal("Consulta inicial", _servico.Nome.Valor);
    }
}
