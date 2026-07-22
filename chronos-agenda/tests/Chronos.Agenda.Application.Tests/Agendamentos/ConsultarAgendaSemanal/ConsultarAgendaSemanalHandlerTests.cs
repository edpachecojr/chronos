using Chronos.Agenda.Application.Agendamentos.ConsultarAgendaSemanal;
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

namespace Chronos.Agenda.Application.Tests.Agendamentos.ConsultarAgendaSemanal;

public class ConsultarAgendaSemanalHandlerTests
{
    private static readonly DateTime CriadoEmUtc = new(2026, 7, 22, 12, 0, 0, DateTimeKind.Utc);
    private static readonly DateOnly InicioDaSemana = new(2026, 7, 27);

    private readonly Guid _organizacaoId;
    private readonly FakeProfissionalRepositorio _profissionalRepositorio = new();
    private readonly FakeOrganizacaoRepositorio _organizacaoRepositorio = new();
    private readonly FakeDisponibilidadeSemanalRepositorio _disponibilidadeRepositorio = new();
    private readonly FakeAgendamentoRepositorio _agendamentoRepositorio = new();
    private readonly FakeProvedorDataHora _provedorDataHora = new(CriadoEmUtc);
    private readonly Profissional _profissional;
    private readonly ConsultarAgendaSemanalHandler _handler;

    public ConsultarAgendaSemanalHandlerTests()
    {
        var organizacao = Organizacao.Criar(new NomeOrganizacao("Clínica Bem-Estar"), _provedorDataHora);
        organizacao.ConfigurarPerfilOperacional(null, new FusoHorario("America/Sao_Paulo"), _provedorDataHora);
        _organizacaoRepositorio.AdicionarAsync(organizacao, CancellationToken.None).GetAwaiter().GetResult();
        _organizacaoId = organizacao.Id;

        _profissional = Profissional.Criar(_organizacaoId, new Nome("Dra. Ana Souza"), _provedorDataHora);
        _profissionalRepositorio.AdicionarAsync(_profissional, CancellationToken.None).GetAwaiter().GetResult();

        var disponibilidade = DisponibilidadeSemanal.Criar(
            _organizacaoId, _profissional.Id, InicioDaSemana.DayOfWeek, new JanelaHorario(new TimeOnly(9, 0), new TimeOnly(18, 0)), _provedorDataHora);
        _disponibilidadeRepositorio.AdicionarAsync(disponibilidade, CancellationToken.None).GetAwaiter().GetResult();

        _handler = new ConsultarAgendaSemanalHandler(
            _profissionalRepositorio, _organizacaoRepositorio, _disponibilidadeRepositorio, _agendamentoRepositorio,
            new ContextoUsuario(Guid.NewGuid(), _organizacaoId));
    }

    [Fact]
    public async Task ExecutarAsync_retorna_sete_dias_a_partir_do_inicio_da_semana()
    {
        var inicioLocal = new DateTimeOffset(InicioDaSemana.Year, InicioDaSemana.Month, InicioDaSemana.Day, 10, 0, 0, TimeSpan.FromHours(-3));
        var periodo = new PeriodoAgendamento(inicioLocal.UtcDateTime, inicioLocal.UtcDateTime.AddMinutes(50));
        var agendamento = Agendamento.Criar(
            _organizacaoId, _profissional.Id, Guid.NewGuid(), "Consulta inicial",
            new PessoaAtendida(new Nome("Maria Silva"), TipoPessoaAtendida.Paciente),
            periodo, new PrecoServico(250m), LocalAtendimento.Online(), _provedorDataHora);
        await _agendamentoRepositorio.AdicionarAsync(agendamento, CancellationToken.None);

        var resultado = await _handler.ExecutarAsync(new ConsultarAgendaSemanalConsulta(_profissional.Id, InicioDaSemana), CancellationToken.None);

        Assert.True(resultado.Sucesso);
        var dias = resultado.Valor.Dias;
        Assert.Equal(7, dias.Count);
        Assert.Equal(InicioDaSemana, dias.First().Data);
        Assert.Equal(InicioDaSemana.AddDays(6), dias.Last().Data);
        var primeiroDia = dias.First();
        Assert.Single(primeiroDia.JanelasDisponiveis);
        var ocupado = Assert.Single(primeiroDia.PeriodosOcupados);
        Assert.Equal(agendamento.Id, ocupado.AgendamentoId);
        Assert.All(dias.Skip(1), dia => Assert.Empty(dia.PeriodosOcupados));
    }

    [Fact]
    public async Task ExecutarAsync_com_profissional_inexistente_retorna_falha()
    {
        var resultado = await _handler.ExecutarAsync(new ConsultarAgendaSemanalConsulta(Guid.NewGuid(), InicioDaSemana), CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Profissional.NaoEncontrado", resultado.Erro!.Codigo);
    }
}
