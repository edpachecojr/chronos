using Chronos.Agenda.Application.Agendamentos.CriarAgendamento;
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

namespace Chronos.Agenda.Application.Tests.Agendamentos.CriarAgendamento;

public class CriarAgendamentoHandlerTests
{
    private static readonly DateTime CriadoEmUtc = new(2026, 7, 22, 12, 0, 0, DateTimeKind.Utc);
    private static readonly DateTimeOffset Inicio = new(2026, 7, 27, 10, 0, 0, TimeSpan.FromHours(-3));

    private readonly Guid _organizacaoId;
    private readonly FakeAgendamentoRepositorio _agendamentoRepositorio = new();
    private readonly FakeProfissionalRepositorio _profissionalRepositorio = new();
    private readonly FakeServicoRepositorio _servicoRepositorio = new();
    private readonly FakeOrganizacaoRepositorio _organizacaoRepositorio = new();
    private readonly FakeDisponibilidadeSemanalRepositorio _disponibilidadeRepositorio = new();
    private readonly FakeUnidadeDeTrabalho _unidadeDeTrabalho = new();
    private readonly FakeProvedorDataHora _provedorDataHora = new(CriadoEmUtc);
    private readonly CriarAgendamentoHandler _handler;
    private readonly Profissional _profissional;
    private readonly Servico _servico;

    public CriarAgendamentoHandlerTests()
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

        var disponibilidade = DisponibilidadeSemanal.Criar(
            _organizacaoId, _profissional.Id, Inicio.DayOfWeek, new JanelaHorario(new TimeOnly(9, 0), new TimeOnly(18, 0)), _provedorDataHora);
        _disponibilidadeRepositorio.AdicionarAsync(disponibilidade, CancellationToken.None).GetAwaiter().GetResult();

        _handler = new CriarAgendamentoHandler(
            _agendamentoRepositorio, _profissionalRepositorio, _servicoRepositorio, _organizacaoRepositorio, _disponibilidadeRepositorio,
            new ContextoUsuario(Guid.NewGuid(), _organizacaoId), _unidadeDeTrabalho, _provedorDataHora);
    }

    private CriarAgendamentoComando ComandoValido(string? enderecoPessoaAtendida = null) =>
        new(_profissional.Id, _servico.Id, "Maria Silva", TipoPessoaAtendida.Paciente, Inicio, enderecoPessoaAtendida);

    [Fact]
    public async Task ExecutarAsync_com_dados_validos_cria_agendamento_pendente()
    {
        var resultado = await _handler.ExecutarAsync(ComandoValido(), CancellationToken.None);

        Assert.True(resultado.Sucesso);
        var persistido = await _agendamentoRepositorio.BuscarPorIdAsync(_organizacaoId, resultado.Valor.AgendamentoId, CancellationToken.None);
        Assert.NotNull(persistido);
        Assert.Equal(StatusAgendamento.Pendente, persistido.Status);
        Assert.Equal(Inicio.UtcDateTime, persistido.Periodo.InicioUtc);
        Assert.Equal(1, _unidadeDeTrabalho.VezesSalvo);
    }

    [Fact]
    public async Task ExecutarAsync_com_profissional_inexistente_retorna_falha()
    {
        var comando = ComandoValido() with { ProfissionalId = Guid.NewGuid() };

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Profissional.NaoEncontrado", resultado.Erro!.Codigo);
    }

    [Fact]
    public async Task ExecutarAsync_com_servico_inexistente_retorna_falha()
    {
        var comando = ComandoValido() with { ServicoId = Guid.NewGuid() };

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Servico.NaoEncontrado", resultado.Erro!.Codigo);
    }

    [Fact]
    public async Task ExecutarAsync_com_servico_de_outro_profissional_retorna_falha_RN04()
    {
        var outroProfissional = Profissional.Criar(_organizacaoId, new Nome("Dr. João Lima"), _provedorDataHora);
        await _profissionalRepositorio.AdicionarAsync(outroProfissional, CancellationToken.None);
        var comando = ComandoValido() with { ProfissionalId = outroProfissional.Id };

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Agendamento.ServicoNaoPertenceAoProfissional", resultado.Erro!.Codigo);
    }

    [Fact]
    public async Task ExecutarAsync_com_organizacao_sem_perfil_operacional_configurado_retorna_falha()
    {
        var organizacaoSemPerfil = Organizacao.Criar(new NomeOrganizacao("Sem Perfil"), _provedorDataHora);
        var profissionalSemPerfil = Profissional.Criar(organizacaoSemPerfil.Id, new Nome("Dra. Ana Souza"), _provedorDataHora);
        var servicoSemPerfil = Servico.Criar(
            organizacaoSemPerfil.Id, profissionalSemPerfil.Id, new NomeServico("Consulta"), new DuracaoServico(TimeSpan.FromMinutes(50)),
            new PrecoServico(250m), TipoAtendimento.Online, _provedorDataHora);
        var organizacaoRepositorio = new FakeOrganizacaoRepositorio();
        await organizacaoRepositorio.AdicionarAsync(organizacaoSemPerfil, CancellationToken.None);
        var profissionalRepositorio = new FakeProfissionalRepositorio();
        await profissionalRepositorio.AdicionarAsync(profissionalSemPerfil, CancellationToken.None);
        var servicoRepositorio = new FakeServicoRepositorio();
        await servicoRepositorio.AdicionarAsync(servicoSemPerfil, CancellationToken.None);
        var handler = new CriarAgendamentoHandler(
            _agendamentoRepositorio, profissionalRepositorio, servicoRepositorio, organizacaoRepositorio, _disponibilidadeRepositorio,
            new ContextoUsuario(Guid.NewGuid(), organizacaoSemPerfil.Id), _unidadeDeTrabalho, _provedorDataHora);
        var comando = new CriarAgendamentoComando(profissionalSemPerfil.Id, servicoSemPerfil.Id, "Maria Silva", TipoPessoaAtendida.Paciente, Inicio, null);

        var resultado = await handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Agendamento.PerfilOperacionalNaoConfigurado", resultado.Erro!.Codigo);
    }

    [Fact]
    public async Task ExecutarAsync_com_atendimento_domiciliar_sem_endereco_retorna_falha()
    {
        var servicoDomiciliar = Servico.Criar(
            _organizacaoId, _profissional.Id, new NomeServico("Fisioterapia domiciliar"), new DuracaoServico(TimeSpan.FromMinutes(50)),
            new PrecoServico(250m), TipoAtendimento.Domiciliar, _provedorDataHora);
        await _servicoRepositorio.AdicionarAsync(servicoDomiciliar, CancellationToken.None);
        var comando = ComandoValido() with { ServicoId = servicoDomiciliar.Id };

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Agendamento.EnderecoObrigatorioAusente", resultado.Erro!.Codigo);
    }

    [Fact]
    public async Task ExecutarAsync_com_atendimento_domiciliar_com_endereco_valido_cria_agendamento()
    {
        var servicoDomiciliar = Servico.Criar(
            _organizacaoId, _profissional.Id, new NomeServico("Fisioterapia domiciliar"), new DuracaoServico(TimeSpan.FromMinutes(50)),
            new PrecoServico(250m), TipoAtendimento.Domiciliar, _provedorDataHora);
        await _servicoRepositorio.AdicionarAsync(servicoDomiciliar, CancellationToken.None);
        var comando = ComandoValido("Rua Exemplo, 10") with { ServicoId = servicoDomiciliar.Id };

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Sucesso);
    }

    [Fact]
    public async Task ExecutarAsync_fora_da_disponibilidade_do_profissional_retorna_falha_RN07()
    {
        var disponibilidadeRepositorio = new FakeDisponibilidadeSemanalRepositorio();
        var handler = new CriarAgendamentoHandler(
            _agendamentoRepositorio, _profissionalRepositorio, _servicoRepositorio, _organizacaoRepositorio, disponibilidadeRepositorio,
            new ContextoUsuario(Guid.NewGuid(), _organizacaoId), _unidadeDeTrabalho, _provedorDataHora);

        var resultado = await handler.ExecutarAsync(ComandoValido(), CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Disponibilidade.ForaDaJanela", resultado.Erro!.Codigo);
    }

    [Fact]
    public async Task ExecutarAsync_com_conflito_de_agenda_ativo_retorna_falha_RN02()
    {
        var periodoExistente = PeriodoAgendamento.APartirDaDuracao(Inicio.UtcDateTime, _servico.Duracao);
        var agendamentoExistente = Agendamento.Criar(
            _organizacaoId, _profissional.Id, _servico.Id, _servico.Nome.Valor,
            new PessoaAtendida(new Nome("Outro Paciente"), TipoPessoaAtendida.Paciente),
            periodoExistente, _servico.Preco, LocalAtendimento.Online(), _provedorDataHora);
        await _agendamentoRepositorio.AdicionarAsync(agendamentoExistente, CancellationToken.None);

        var resultado = await _handler.ExecutarAsync(ComandoValido(), CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Agendamento.ConflitoDeAgenda", resultado.Erro!.Codigo);
    }

    [Fact]
    public async Task ExecutarAsync_com_periodo_atravessando_meia_noite_retorna_falha()
    {
        var inicioProximoAMeiaNoite = new DateTimeOffset(2026, 7, 27, 23, 40, 0, TimeSpan.FromHours(-3));
        var comando = ComandoValido() with { Inicio = inicioProximoAMeiaNoite };

        var resultado = await _handler.ExecutarAsync(comando, CancellationToken.None);

        Assert.True(resultado.Falhou);
        Assert.Equal("Agendamento.PeriodoAtravessaMeiaNoite", resultado.Erro!.Codigo);
    }
}
