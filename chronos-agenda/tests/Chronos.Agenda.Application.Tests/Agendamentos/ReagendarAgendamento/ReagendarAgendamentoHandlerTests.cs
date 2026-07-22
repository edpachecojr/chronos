using Chronos.Agenda.Application.Agendamentos.ReagendarAgendamento;
using Chronos.Agenda.Application.Compartilhado;
using Chronos.Agenda.Application.Tests.Agendamentos.Fakes;
using Chronos.Agenda.Application.Tests.Compartilhado.Fakes;
using Chronos.Agenda.Application.Tests.Disponibilidades.Fakes;
using Chronos.Agenda.Application.Tests.Organizacoes.Fakes;
using Chronos.Agenda.Application.Tests.Profissionais.Fakes;
using Chronos.Agenda.Application.Tests.Servicos.Fakes;
using Chronos.Agenda.Domain.Agendamentos.Entidades;
using Chronos.Agenda.Domain.Agendamentos.Enums;
using Chronos.Agenda.Domain.Agendamentos.ObjetosValor;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Disponibilidades.Entidades;
using Chronos.Agenda.Domain.Disponibilidades.ObjetosValor;
using Chronos.Agenda.Domain.Organizacoes.Entidades;
using Chronos.Agenda.Domain.Organizacoes.ObjetosValor;
using Chronos.Agenda.Domain.Profissionais.Entidades;
using Chronos.Agenda.Domain.Servicos.Entidades;
using Chronos.Agenda.Domain.Servicos.Enums;
using Chronos.Agenda.Domain.Servicos.ObjetosValor;

namespace Chronos.Agenda.Application.Tests.Agendamentos.ReagendarAgendamento;

public class ReagendarAgendamentoHandlerTests
{
    private static readonly DateTime CriadoEmUtc = new(2026, 7, 22, 12, 0, 0, DateTimeKind.Utc);
    private static readonly DateTimeOffset InicioOriginal = new(2026, 7, 27, 10, 0, 0, TimeSpan.FromHours(-3));
    private static readonly DateTimeOffset NovoInicio = new(2026, 7, 28, 14, 0, 0, TimeSpan.FromHours(-3));

    private readonly Guid _organizacaoId;
    private readonly FakeAgendamentoRepositorio _agendamentoRepositorio = new();
    private readonly FakeProfissionalRepositorio _profissionalRepositorio = new();
    private readonly FakeServicoRepositorio _servicoRepositorio = new();
    private readonly FakeOrganizacaoRepositorio _organizacaoRepositorio = new();
    private readonly FakeDisponibilidadeSemanalRepositorio _disponibilidadeRepositorio = new();
    private readonly FakeUnidadeDeTrabalho _unidadeDeTrabalho = new();
    private readonly FakeProvedorDataHora _provedorDataHora = new(CriadoEmUtc);
    private readonly ReagendarAgendamentoHandler _handler;
    private readonly Profissional _profissional;
    private readonly Servico _servico;
    private readonly Agendamento _agendamentoExistente;

    public ReagendarAgendamentoHandlerTests()
    {
        var organizacao = Organizacao.Criar(new NomeOrganizacao("Clínica Bem-Estar"), _provedorDataHora);
        organizacao.ConfigurarPerfilOperacional(null, new FusoHorario("America/Sao_Paulo"), _provedorDataHora);
        _organizacaoRepositorio.AdicionarAsync(organizacao, CancellationToken.None).GetAwaiter().GetResult();
        _organizacaoId = organizacao.Id;

        _profissional = Profissional.Criar(_organizacaoId, new Nome("Dra. Ana Souza"), _provedorDataHora);
        _profissionalRepositorio.AdicionarAsync(_profissional, CancellationToken.None).GetAwaiter().GetResult();

        _servico = Servico.Criar(
            _organizacaoId, _profissional.Id, new NomeServico("Consulta inicial"), new DuracaoServico(TimeSpan.FromMinutes(50)),
            new PrecoServico(250m), TipoAtendimento.Online, _provedorDataHora);
        _servicoRepositorio.AdicionarAsync(_servico, CancellationToken.None).GetAwaiter().GetResult();

        foreach (var diaDaSemana in new[] { InicioOriginal.DayOfWeek, NovoInicio.DayOfWeek })
        {
            var disponibilidade = DisponibilidadeSemanal.Criar(
                _organizacaoId, _profissional.Id, diaDaSemana, new JanelaHorario(new TimeOnly(9, 0), new TimeOnly(18, 0)), _provedorDataHora);
            _disponibilidadeRepositorio.AdicionarAsync(disponibilidade, CancellationToken.None).GetAwaiter().GetResult();
        }

        var periodoOriginal = PeriodoAgendamento.APartirDaDuracao(InicioOriginal.UtcDateTime, _servico.Duracao);
        _agendamentoExistente = Agendamento.Criar(
            _organizacaoId, _profissional.Id, _servico.Id, _servico.Nome.Valor,
            new PessoaAtendida(new Nome("Maria Silva"), TipoPessoaAtendida.Paciente),
            periodoOriginal, _servico.Preco, LocalAtendimento.Online(), _provedorDataHora);
        _agendamentoRepositorio.AdicionarAsync(_agendamentoExistente, CancellationToken.None).GetAwaiter().GetResult();

        _handler = new ReagendarAgendamentoHandler(
            _agendamentoRepositorio, _profissionalRepositorio, _servicoRepositorio, _organizacaoRepositorio, _disponibilidadeRepositorio,
            new ContextoUsuario(Guid.NewGuid(), _organizacaoId), _unidadeDeTrabalho, _provedorDataHora);
    }

    private ReagendarAgendamentoComando ComandoValido(DateTimeOffset? inicio = null) => new(
        _agendamentoExistente.Id, _profissional.Id, _servico.Id, "Maria Silva", TipoPessoaAtendida.Paciente, inicio ?? NovoInicio, null);

    [Fact]
    public async Task ExecutarAsync_com_dados_validos_reagenda_o_periodo()
    {
        var resultado = await _handler.ExecutarAsync(ComandoValido(), CancellationToken.None);

        Assert.True(resultado.Sucesso);
        var persistido = await _agendamentoRepositorio.BuscarPorIdAsync(_organizacaoId, _agendamentoExistente.Id, CancellationToken.None);
        Assert.Equal(NovoInicio.UtcDateTime, persistido!.Periodo.InicioUtc);
        Assert.Equal(1, _unidadeDeTrabalho.VezesSalvo);
    }

    [Fact]
    public async Task ExecutarAsync_com_agendamento_inexistente_retorna_falha()
    {
        var comando = ComandoValido() with { AgendamentoId = Guid.NewGuid() };

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Agendamento.NaoEncontrado", resultado.Erro!.Codigo);
    }

    [Fact]
    public async Task ExecutarAsync_trocando_o_profissional_retorna_falha()
    {
        var outroProfissional = Profissional.Criar(_organizacaoId, new Nome("Dr. João Lima"), _provedorDataHora);
        await _profissionalRepositorio.AdicionarAsync(outroProfissional, CancellationToken.None);
        var comando = ComandoValido() with { ProfissionalId = outroProfissional.Id };

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Agendamento.AlteracaoDeProfissionalOuServicoNaoPermitida", resultado.Erro!.Codigo);
    }

    [Fact]
    public async Task ExecutarAsync_nao_conflita_com_o_proprio_periodo_original()
    {
        var comando = ComandoValido(InicioOriginal.AddMinutes(10));

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Sucesso);
    }

    [Fact]
    public async Task ExecutarAsync_com_conflito_contra_outro_agendamento_ativo_retorna_falha_RN02()
    {
        var periodoOutro = PeriodoAgendamento.APartirDaDuracao(NovoInicio.UtcDateTime, _servico.Duracao);
        var outroAgendamento = Agendamento.Criar(
            _organizacaoId, _profissional.Id, _servico.Id, _servico.Nome.Valor,
            new PessoaAtendida(new Nome("Outro Paciente"), TipoPessoaAtendida.Paciente),
            periodoOutro, _servico.Preco, LocalAtendimento.Online(), _provedorDataHora);
        await _agendamentoRepositorio.AdicionarAsync(outroAgendamento, CancellationToken.None);

        var resultado = await _handler.ExecutarAsync(ComandoValido(), CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Agendamento.ConflitoDeAgenda", resultado.Erro!.Codigo);
    }

    [Fact]
    public async Task ExecutarAsync_fora_da_disponibilidade_do_profissional_retorna_falha_RN07()
    {
        var comando = ComandoValido(new DateTimeOffset(2026, 7, 28, 23, 40, 0, TimeSpan.FromHours(-3)));

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Agendamento.PeriodoAtravessaMeiaNoite", resultado.Erro!.Codigo);
    }

    [Fact]
    public async Task ExecutarAsync_sobre_agendamento_ja_cancelado_retorna_falha()
    {
        _agendamentoExistente.Cancelar(_provedorDataHora);
        await _agendamentoRepositorio.AtualizarAsync(_agendamentoExistente, CancellationToken.None);

        var resultado = await _handler.ExecutarAsync(ComandoValido(), CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Agendamento.JaCancelado", resultado.Erro!.Codigo);
    }
}
