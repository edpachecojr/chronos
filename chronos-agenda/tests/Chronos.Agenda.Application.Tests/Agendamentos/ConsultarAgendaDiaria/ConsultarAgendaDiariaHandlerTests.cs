using Chronos.Agenda.Application.Agendamentos.ConsultarAgendaDiaria;
using Chronos.Agenda.Application.Compartilhado;
using Chronos.Agenda.Application.Tests.Agendamentos.Fakes;
using Chronos.Agenda.Application.Tests.Compartilhado.Fakes;
using Chronos.Agenda.Application.Tests.Disponibilidades.Fakes;
using Chronos.Agenda.Application.Tests.Organizacoes.Fakes;
using Chronos.Agenda.Application.Tests.Profissionais.Fakes;
using Chronos.Agenda.Domain.Agendamentos.Entidades;
using Chronos.Agenda.Domain.Agendamentos.Enums;
using Chronos.Agenda.Domain.Agendamentos.ObjetosValor;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Disponibilidades.Entidades;
using Chronos.Agenda.Domain.Disponibilidades.ObjetosValor;
using Chronos.Agenda.Domain.Organizacoes.Entidades;
using Chronos.Agenda.Domain.Organizacoes.ObjetosValor;
using Chronos.Agenda.Domain.Profissionais.Entidades;
using Chronos.Agenda.Domain.Servicos.ObjetosValor;

namespace Chronos.Agenda.Application.Tests.Agendamentos.ConsultarAgendaDiaria;

public class ConsultarAgendaDiariaHandlerTests
{
    private static readonly DateTime CriadoEmUtc = new(2026, 7, 22, 12, 0, 0, DateTimeKind.Utc);
    private static readonly DateOnly DataConsultada = new(2026, 7, 27);

    private readonly Guid _organizacaoId;
    private readonly FakeProfissionalRepositorio _profissionalRepositorio = new();
    private readonly FakeOrganizacaoRepositorio _organizacaoRepositorio = new();
    private readonly FakeDisponibilidadeSemanalRepositorio _disponibilidadeRepositorio = new();
    private readonly FakeAgendamentoRepositorio _agendamentoRepositorio = new();
    private readonly FakeProvedorDataHora _provedorDataHora = new(CriadoEmUtc);
    private readonly Organizacao _organizacao;
    private readonly Profissional _profissional;
    private readonly ConsultarAgendaDiariaHandler _handler;

    public ConsultarAgendaDiariaHandlerTests()
    {
        _organizacao = Organizacao.Criar(new NomeOrganizacao("Clínica Bem-Estar"), _provedorDataHora);
        _organizacao.ConfigurarPerfilOperacional(null, new FusoHorario("America/Sao_Paulo"), _provedorDataHora);
        _organizacaoRepositorio.AdicionarAsync(_organizacao, CancellationToken.None).GetAwaiter().GetResult();
        _organizacaoId = _organizacao.Id;

        _profissional = Profissional.Criar(_organizacaoId, new Nome("Dra. Ana Souza"), _provedorDataHora);
        _profissionalRepositorio.AdicionarAsync(_profissional, CancellationToken.None).GetAwaiter().GetResult();

        var disponibilidade = DisponibilidadeSemanal.Criar(
            _organizacaoId, _profissional.Id, DataConsultada.DayOfWeek, new JanelaHorario(new TimeOnly(9, 0), new TimeOnly(18, 0)), _provedorDataHora);
        _disponibilidadeRepositorio.AdicionarAsync(disponibilidade, CancellationToken.None).GetAwaiter().GetResult();

        _handler = new ConsultarAgendaDiariaHandler(
            _profissionalRepositorio, _organizacaoRepositorio, _disponibilidadeRepositorio, _agendamentoRepositorio,
            new ContextoUsuario(Guid.NewGuid(), _organizacaoId));
    }

    private Agendamento CriarAgendamento(DateTimeOffset inicioLocal, StatusAgendamento status)
    {
        var periodo = new PeriodoAgendamento(inicioLocal.UtcDateTime, inicioLocal.UtcDateTime.AddMinutes(50));
        var agendamento = Agendamento.Criar(
            _organizacaoId, _profissional.Id, Guid.NewGuid(), "Consulta inicial",
            new PessoaAtendida(new Nome("Maria Silva"), TipoPessoaAtendida.Paciente),
            periodo, new PrecoServico(250m), LocalAtendimento.Online(), _provedorDataHora);
        if (status == StatusAgendamento.Confirmado)
        {
            agendamento.Confirmar(_provedorDataHora);
        }
        else if (status == StatusAgendamento.Cancelado)
        {
            agendamento.Cancelar(_provedorDataHora);
        }

        return agendamento;
    }

    [Fact]
    public async Task ExecutarAsync_projeta_janelas_e_periodos_ocupados_do_dia()
    {
        var inicioLocal = new DateTimeOffset(DataConsultada.Year, DataConsultada.Month, DataConsultada.Day, 10, 0, 0, TimeSpan.FromHours(-3));
        var ativo = CriarAgendamento(inicioLocal, StatusAgendamento.Pendente);
        await _agendamentoRepositorio.AdicionarAsync(ativo, CancellationToken.None);

        var resultado = await _handler.ExecutarAsync(new ConsultarAgendaDiariaConsulta(_profissional.Id, DataConsultada), CancellationToken.None);

        Assert.True(resultado.Sucesso);
        var agenda = resultado.Valor;
        Assert.Equal(DataConsultada, agenda.Data);
        Assert.Single(agenda.JanelasDisponiveis);
        Assert.Equal(new TimeOnly(9, 0), agenda.JanelasDisponiveis.Single().Inicio);
        var ocupado = Assert.Single(agenda.PeriodosOcupados);
        Assert.Equal(ativo.Id, ocupado.AgendamentoId);
        Assert.Equal(ativo.ServicoId, ocupado.ServicoId);
        Assert.Equal(new TimeOnly(10, 0), ocupado.Inicio);
        Assert.Equal(new TimeOnly(10, 50), ocupado.Fim);
        Assert.Equal(StatusAgendamento.Pendente, ocupado.Status);
        Assert.Equal("Consulta inicial", ocupado.NomeServico);
        Assert.Equal("Maria Silva", ocupado.NomePessoaAtendida);
    }

    [Fact]
    public async Task ExecutarAsync_ignora_agendamentos_cancelados()
    {
        var inicioLocal = new DateTimeOffset(DataConsultada.Year, DataConsultada.Month, DataConsultada.Day, 10, 0, 0, TimeSpan.FromHours(-3));
        var cancelado = CriarAgendamento(inicioLocal, StatusAgendamento.Cancelado);
        await _agendamentoRepositorio.AdicionarAsync(cancelado, CancellationToken.None);

        var resultado = await _handler.ExecutarAsync(new ConsultarAgendaDiariaConsulta(_profissional.Id, DataConsultada), CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.Empty(resultado.Valor.PeriodosOcupados);
    }

    [Fact]
    public async Task ExecutarAsync_ignora_agendamentos_de_outro_dia()
    {
        var inicioOutroDia = new DateTimeOffset(DataConsultada.Year, DataConsultada.Month, DataConsultada.Day, 10, 0, 0, TimeSpan.FromHours(-3)).AddDays(1);
        var deOutroDia = CriarAgendamento(inicioOutroDia, StatusAgendamento.Pendente);
        await _agendamentoRepositorio.AdicionarAsync(deOutroDia, CancellationToken.None);

        var resultado = await _handler.ExecutarAsync(new ConsultarAgendaDiariaConsulta(_profissional.Id, DataConsultada), CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.Empty(resultado.Valor.PeriodosOcupados);
    }

    [Fact]
    public async Task ExecutarAsync_com_profissional_inexistente_retorna_falha()
    {
        var resultado = await _handler.ExecutarAsync(new ConsultarAgendaDiariaConsulta(Guid.NewGuid(), DataConsultada), CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Profissional.NaoEncontrado", resultado.Erro!.Codigo);
    }

    [Fact]
    public async Task ExecutarAsync_com_organizacao_sem_perfil_operacional_retorna_falha()
    {
        var organizacaoSemPerfil = Organizacao.Criar(new NomeOrganizacao("Sem Perfil"), _provedorDataHora);
        var profissionalSemPerfil = Profissional.Criar(organizacaoSemPerfil.Id, new Nome("Dr. João Lima"), _provedorDataHora);
        var organizacaoRepositorio = new FakeOrganizacaoRepositorio();
        await organizacaoRepositorio.AdicionarAsync(organizacaoSemPerfil, CancellationToken.None);
        var profissionalRepositorio = new FakeProfissionalRepositorio();
        await profissionalRepositorio.AdicionarAsync(profissionalSemPerfil, CancellationToken.None);
        var handler = new ConsultarAgendaDiariaHandler(
            profissionalRepositorio, organizacaoRepositorio, _disponibilidadeRepositorio, _agendamentoRepositorio,
            new ContextoUsuario(Guid.NewGuid(), organizacaoSemPerfil.Id));

        var resultado = await handler.ExecutarAsync(new ConsultarAgendaDiariaConsulta(profissionalSemPerfil.Id, DataConsultada), CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Agendamento.PerfilOperacionalNaoConfigurado", resultado.Erro!.Codigo);
    }
}
