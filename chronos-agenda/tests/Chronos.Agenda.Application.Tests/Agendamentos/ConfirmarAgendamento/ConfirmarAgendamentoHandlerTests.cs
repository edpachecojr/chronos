using Chronos.Agenda.Application.Agendamentos.ConfirmarAgendamento;
using Chronos.Agenda.Application.Compartilhado;
using Chronos.Agenda.Application.Tests.Agendamentos.Fakes;
using Chronos.Agenda.Application.Tests.Compartilhado.Fakes;
using Chronos.Agenda.Domain.Agendamentos.Entidades;
using Chronos.Agenda.Domain.Agendamentos.Enums;
using Chronos.Agenda.Domain.Agendamentos.ObjetosValor;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Servicos.ObjetosValor;

namespace Chronos.Agenda.Application.Tests.Agendamentos.ConfirmarAgendamento;

public class ConfirmarAgendamentoHandlerTests
{
    private static readonly DateTime CriadoEmUtc = new(2026, 7, 22, 12, 0, 0, DateTimeKind.Utc);

    private readonly Guid _organizacaoId = Guid.NewGuid();
    private readonly FakeAgendamentoRepositorio _agendamentoRepositorio = new();
    private readonly FakeUnidadeDeTrabalho _unidadeDeTrabalho = new();
    private readonly FakeProvedorDataHora _provedorDataHora = new(CriadoEmUtc);
    private readonly ConfirmarAgendamentoHandler _handler;

    public ConfirmarAgendamentoHandlerTests()
    {
        _handler = new ConfirmarAgendamentoHandler(
            _agendamentoRepositorio, new ContextoUsuario(Guid.NewGuid(), _organizacaoId), _unidadeDeTrabalho, _provedorDataHora);
    }

    private Agendamento CriarAgendamentoPendente()
    {
        var periodo = new PeriodoAgendamento(CriadoEmUtc.AddDays(1), CriadoEmUtc.AddDays(1).AddMinutes(50));
        return Agendamento.Criar(
            _organizacaoId, Guid.NewGuid(), Guid.NewGuid(), "Consulta inicial",
            new PessoaAtendida(new Nome("Maria Silva"), TipoPessoaAtendida.Paciente),
            periodo, new PrecoServico(250m), LocalAtendimento.Online(), _provedorDataHora);
    }

    [Fact]
    public async Task ExecutarAsync_com_agendamento_pendente_confirma()
    {
        var agendamento = CriarAgendamentoPendente();
        await _agendamentoRepositorio.AdicionarAsync(agendamento, CancellationToken.None);

        var resultado = await _handler.ExecutarAsync(new ConfirmarAgendamentoComando(agendamento.Id), CancellationToken.None);

        Assert.True(resultado.Sucesso);
        var persistido = await _agendamentoRepositorio.BuscarPorIdAsync(_organizacaoId, agendamento.Id, CancellationToken.None);
        Assert.Equal(StatusAgendamento.Confirmado, persistido!.Status);
        Assert.Equal(1, _unidadeDeTrabalho.VezesSalvo);
    }

    [Fact]
    public async Task ExecutarAsync_com_agendamento_inexistente_retorna_falha()
    {
        var resultado = await _handler.ExecutarAsync(new ConfirmarAgendamentoComando(Guid.NewGuid()), CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Agendamento.NaoEncontrado", resultado.Erro!.Codigo);
    }

    [Fact]
    public async Task ExecutarAsync_com_agendamento_ja_confirmado_retorna_falha()
    {
        var agendamento = CriarAgendamentoPendente();
        agendamento.Confirmar(_provedorDataHora);
        await _agendamentoRepositorio.AdicionarAsync(agendamento, CancellationToken.None);

        var resultado = await _handler.ExecutarAsync(new ConfirmarAgendamentoComando(agendamento.Id), CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Agendamento.ConfirmacaoInvalida", resultado.Erro!.Codigo);
    }

    [Fact]
    public async Task ExecutarAsync_com_agendamento_cancelado_retorna_falha()
    {
        var agendamento = CriarAgendamentoPendente();
        agendamento.Cancelar(_provedorDataHora);
        await _agendamentoRepositorio.AdicionarAsync(agendamento, CancellationToken.None);

        var resultado = await _handler.ExecutarAsync(new ConfirmarAgendamentoComando(agendamento.Id), CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Agendamento.ConfirmacaoInvalida", resultado.Erro!.Codigo);
    }
}
