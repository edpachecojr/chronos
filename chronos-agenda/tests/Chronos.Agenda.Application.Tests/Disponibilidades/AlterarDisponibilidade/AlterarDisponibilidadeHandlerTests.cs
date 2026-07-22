using Chronos.Agenda.Application.Compartilhado;
using Chronos.Agenda.Application.Disponibilidades.AlterarDisponibilidade;
using Chronos.Agenda.Application.Tests.Compartilhado.Fakes;
using Chronos.Agenda.Application.Tests.Disponibilidades.Fakes;
using Chronos.Agenda.Application.Tests.Profissionais.Fakes;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Disponibilidades.Entidades;
using Chronos.Agenda.Domain.Disponibilidades.ObjetosValor;
using Chronos.Agenda.Domain.Profissionais.Entidades;

namespace Chronos.Agenda.Application.Tests.Disponibilidades.AlterarDisponibilidade;

public class AlterarDisponibilidadeHandlerTests
{
    private static readonly DateTime CriadoEmUtc = new(2026, 7, 22, 12, 0, 0, DateTimeKind.Utc);

    private readonly Guid _organizacaoId = Guid.NewGuid();
    private readonly FakeDisponibilidadeSemanalRepositorio _disponibilidadeRepositorio = new();
    private readonly FakeProfissionalRepositorio _profissionalRepositorio = new();
    private readonly FakeUnidadeDeTrabalho _unidadeDeTrabalho = new();
    private readonly AlterarDisponibilidadeHandler _handler;
    private readonly Profissional _profissional;
    private readonly DisponibilidadeSemanal _existente;

    public AlterarDisponibilidadeHandlerTests()
    {
        var provedorDataHora = new FakeProvedorDataHora(CriadoEmUtc);
        _profissional = Profissional.Criar(_organizacaoId, new Nome("Dra. Ana Souza"), provedorDataHora);
        _profissionalRepositorio.AdicionarAsync(_profissional, CancellationToken.None).GetAwaiter().GetResult();
        _existente = DisponibilidadeSemanal.Criar(
            _organizacaoId, _profissional.Id, DayOfWeek.Monday, new JanelaHorario(new TimeOnly(9, 0), new TimeOnly(12, 0)), provedorDataHora);
        _disponibilidadeRepositorio.AdicionarAsync(_existente, CancellationToken.None).GetAwaiter().GetResult();
        _handler = new AlterarDisponibilidadeHandler(
            _disponibilidadeRepositorio,
            _profissionalRepositorio,
            new ContextoUsuario(Guid.NewGuid(), _organizacaoId),
            _unidadeDeTrabalho,
            provedorDataHora);
    }

    [Fact]
    public async Task ExecutarAsync_com_dados_validos_reagenda_a_disponibilidade()
    {
        var comando = new AlterarDisponibilidadeComando(_profissional.Id, _existente.Id, DayOfWeek.Wednesday, new TimeOnly(14, 0), new TimeOnly(18, 0));

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.Equal(DayOfWeek.Wednesday, _existente.DiaDaSemana);
        Assert.Equal(1, _unidadeDeTrabalho.VezesSalvo);
    }

    [Fact]
    public async Task ExecutarAsync_com_disponibilidade_inexistente_retorna_falha()
    {
        var comando = new AlterarDisponibilidadeComando(_profissional.Id, Guid.NewGuid(), DayOfWeek.Wednesday, new TimeOnly(14, 0), new TimeOnly(18, 0));

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Disponibilidade.NaoEncontrada", resultado.Erro!.Codigo);
    }

    [Fact]
    public async Task ExecutarAsync_com_sobreposicao_a_outra_janela_do_profissional_retorna_falha()
    {
        var outra = DisponibilidadeSemanal.Criar(
            _organizacaoId, _profissional.Id, DayOfWeek.Wednesday, new JanelaHorario(new TimeOnly(14, 0), new TimeOnly(18, 0)),
            new FakeProvedorDataHora(CriadoEmUtc));
        await _disponibilidadeRepositorio.AdicionarAsync(outra, CancellationToken.None);
        var comando = new AlterarDisponibilidadeComando(_profissional.Id, _existente.Id, DayOfWeek.Wednesday, new TimeOnly(15, 0), new TimeOnly(17, 0));

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Disponibilidade.JanelaSobreposta", resultado.Erro!.Codigo);
    }
}
