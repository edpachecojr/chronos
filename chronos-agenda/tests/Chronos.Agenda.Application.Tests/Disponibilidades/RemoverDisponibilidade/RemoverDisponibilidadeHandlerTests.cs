using Chronos.Agenda.Application.Compartilhado;
using Chronos.Agenda.Application.Disponibilidades.RemoverDisponibilidade;
using Chronos.Agenda.Application.Tests.Compartilhado.Fakes;
using Chronos.Agenda.Application.Tests.Disponibilidades.Fakes;
using Chronos.Agenda.Application.Tests.Profissionais.Fakes;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Disponibilidades.Entidades;
using Chronos.Agenda.Domain.Disponibilidades.ObjetosValor;
using Chronos.Agenda.Domain.Profissionais.Entidades;

namespace Chronos.Agenda.Application.Tests.Disponibilidades.RemoverDisponibilidade;

public class RemoverDisponibilidadeHandlerTests
{
    private static readonly DateTime CriadoEmUtc = new(2026, 7, 22, 12, 0, 0, DateTimeKind.Utc);

    private readonly Guid _organizacaoId = Guid.NewGuid();
    private readonly FakeDisponibilidadeSemanalRepositorio _disponibilidadeRepositorio = new();
    private readonly FakeProfissionalRepositorio _profissionalRepositorio = new();
    private readonly FakeUnidadeDeTrabalho _unidadeDeTrabalho = new();
    private readonly RemoverDisponibilidadeHandler _handler;
    private readonly Profissional _profissional;
    private readonly DisponibilidadeSemanal _existente;

    public RemoverDisponibilidadeHandlerTests()
    {
        var provedorDataHora = new FakeProvedorDataHora(CriadoEmUtc);
        _profissional = Profissional.Criar(_organizacaoId, new Nome("Dra. Ana Souza"), provedorDataHora);
        _profissionalRepositorio.AdicionarAsync(_profissional, CancellationToken.None).GetAwaiter().GetResult();
        _existente = DisponibilidadeSemanal.Criar(
            _organizacaoId, _profissional.Id, DayOfWeek.Monday, new JanelaHorario(new TimeOnly(9, 0), new TimeOnly(12, 0)), provedorDataHora);
        _disponibilidadeRepositorio.AdicionarAsync(_existente, CancellationToken.None).GetAwaiter().GetResult();
        _handler = new RemoverDisponibilidadeHandler(
            _disponibilidadeRepositorio,
            _profissionalRepositorio,
            new ContextoUsuario(Guid.NewGuid(), _organizacaoId),
            _unidadeDeTrabalho);
    }

    [Fact]
    public async Task ExecutarAsync_com_disponibilidade_existente_remove()
    {
        var comando = new RemoverDisponibilidadeComando(_profissional.Id, _existente.Id);

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        var restantes = await _disponibilidadeRepositorio.BuscarPorProfissionalAsync(_organizacaoId, _profissional.Id, CancellationToken.None);
        Assert.Empty(restantes);
        Assert.Equal(1, _unidadeDeTrabalho.VezesSalvo);
    }

    [Fact]
    public async Task ExecutarAsync_com_disponibilidade_inexistente_retorna_falha()
    {
        var comando = new RemoverDisponibilidadeComando(_profissional.Id, Guid.NewGuid());

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Disponibilidade.NaoEncontrada", resultado.Erro!.Codigo);
        Assert.Equal(0, _unidadeDeTrabalho.VezesSalvo);
    }

    [Fact]
    public async Task ExecutarAsync_com_profissional_inexistente_retorna_falha()
    {
        var comando = new RemoverDisponibilidadeComando(Guid.NewGuid(), _existente.Id);

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Profissional.NaoEncontrado", resultado.Erro!.Codigo);
    }
}
