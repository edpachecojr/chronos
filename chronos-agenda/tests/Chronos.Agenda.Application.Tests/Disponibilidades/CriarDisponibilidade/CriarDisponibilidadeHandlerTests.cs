using Chronos.Agenda.Application.Compartilhado;
using Chronos.Agenda.Application.Disponibilidades.CriarDisponibilidade;
using Chronos.Agenda.Application.Tests.Compartilhado.Fakes;
using Chronos.Agenda.Application.Tests.Disponibilidades.Fakes;
using Chronos.Agenda.Application.Tests.Profissionais.Fakes;
using Chronos.Agenda.Domain.Profissionais.Entidades;

namespace Chronos.Agenda.Application.Tests.Disponibilidades.CriarDisponibilidade;

public class CriarDisponibilidadeHandlerTests
{
    private static readonly DateTime CriadoEmUtc = new(2026, 7, 22, 12, 0, 0, DateTimeKind.Utc);

    private readonly Guid _organizacaoId = Guid.NewGuid();
    private readonly FakeDisponibilidadeSemanalRepositorio _disponibilidadeRepositorio = new();
    private readonly FakeProfissionalRepositorio _profissionalRepositorio = new();
    private readonly FakeUnidadeDeTrabalho _unidadeDeTrabalho = new();
    private readonly CriarDisponibilidadeHandler _handler;
    private readonly Profissional _profissional;

    public CriarDisponibilidadeHandlerTests()
    {
        var provedorDataHora = new FakeProvedorDataHora(CriadoEmUtc);
        _profissional = Profissional.Criar(_organizacaoId, new Domain.Compartilhado.ObjetosValor.Nome("Dra. Ana Souza"), provedorDataHora);
        _profissionalRepositorio.AdicionarAsync(_profissional, CancellationToken.None).GetAwaiter().GetResult();
        _handler = new CriarDisponibilidadeHandler(
            _disponibilidadeRepositorio,
            _profissionalRepositorio,
            new ContextoUsuario(Guid.NewGuid(), _organizacaoId),
            _unidadeDeTrabalho,
            provedorDataHora);
    }

    [Fact]
    public async Task ExecutarAsync_com_dados_validos_cria_disponibilidade()
    {
        var comando = new CriarDisponibilidadeComando(_profissional.Id, DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(12, 0));

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        var persistidas = await _disponibilidadeRepositorio.BuscarPorProfissionalAsync(_organizacaoId, _profissional.Id, CancellationToken.None);
        Assert.Single(persistidas);
        Assert.Equal(1, _unidadeDeTrabalho.VezesSalvo);
    }

    [Fact]
    public async Task ExecutarAsync_com_profissional_inexistente_retorna_falha()
    {
        var comando = new CriarDisponibilidadeComando(Guid.NewGuid(), DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(12, 0));

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Profissional.NaoEncontrado", resultado.Erro!.Codigo);
        Assert.Equal(0, _unidadeDeTrabalho.VezesSalvo);
    }

    [Fact]
    public async Task ExecutarAsync_com_janela_invalida_retorna_falha()
    {
        var comando = new CriarDisponibilidadeComando(_profissional.Id, DayOfWeek.Monday, new TimeOnly(12, 0), new TimeOnly(9, 0));

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Disponibilidade.JanelaInvalida", resultado.Erro!.Codigo);
    }

    [Fact]
    public async Task ExecutarAsync_com_janela_sobreposta_no_mesmo_dia_retorna_falha_e_nao_persiste()
    {
        await _handler.ExecutarAsync(
            new CriarDisponibilidadeComando(_profissional.Id, DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(12, 0)),
            CancellationToken.None);
        var comandoSobreposto = new CriarDisponibilidadeComando(_profissional.Id, DayOfWeek.Monday, new TimeOnly(11, 0), new TimeOnly(14, 0));

        var resultado = await _handler.ExecutarAsync(comandoSobreposto, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Disponibilidade.JanelaSobreposta", resultado.Erro!.Codigo);
        Assert.Equal(1, _unidadeDeTrabalho.VezesSalvo);
    }
}
