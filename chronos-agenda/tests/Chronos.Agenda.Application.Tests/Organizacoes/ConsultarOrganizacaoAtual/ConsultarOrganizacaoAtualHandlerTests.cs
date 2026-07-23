using Chronos.Agenda.Application.Compartilhado;
using Chronos.Agenda.Application.Organizacoes.ConsultarOrganizacaoAtual;
using Chronos.Agenda.Application.Tests.Compartilhado.Fakes;
using Chronos.Agenda.Application.Tests.Disponibilidades.Fakes;
using Chronos.Agenda.Application.Tests.Organizacoes.Fakes;
using Chronos.Agenda.Application.Tests.Profissionais.Fakes;
using Chronos.Agenda.Application.Tests.Servicos.Fakes;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Disponibilidades.Entidades;
using Chronos.Agenda.Domain.Disponibilidades.ObjetosValor;
using Chronos.Agenda.Domain.Organizacoes.Entidades;
using Chronos.Agenda.Domain.Organizacoes.ObjetosValor;
using Chronos.Agenda.Domain.Profissionais.Entidades;
using Chronos.Agenda.Domain.Servicos.Entidades;
using Chronos.Agenda.Domain.Servicos.Enums;
using Chronos.Agenda.Domain.Servicos.ObjetosValor;

namespace Chronos.Agenda.Application.Tests.Organizacoes.ConsultarOrganizacaoAtual;

public class ConsultarOrganizacaoAtualHandlerTests
{
    private readonly FakeOrganizacaoRepositorio _organizacaoRepositorio = new();
    private readonly FakeProfissionalRepositorio _profissionalRepositorio = new();
    private readonly FakeDisponibilidadeSemanalRepositorio _disponibilidadeRepositorio = new();
    private readonly FakeServicoRepositorio _servicoRepositorio = new();
    private readonly FakeProvedorDataHora _provedorDataHora = new(DateTime.UtcNow);

    private ConsultarOrganizacaoAtualHandler CriarHandler(ContextoUsuario contextoUsuario) => new(
        _organizacaoRepositorio, _profissionalRepositorio, _disponibilidadeRepositorio, _servicoRepositorio, contextoUsuario);

    [Fact]
    public async Task ExecutarAsync_com_usuario_sem_organizacao_retorna_nulo()
    {
        var contextoUsuario = new ContextoUsuario(Guid.NewGuid(), Guid.Empty);
        var handler = CriarHandler(contextoUsuario);

        var resultado = await handler.ExecutarAsync(CancellationToken.None);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task ExecutarAsync_com_usuario_vinculado_a_organizacao_retorna_seus_dados()
    {
        var organizacao = Organizacao.Criar(new NomeOrganizacao("Clínica Bem-Estar"), _provedorDataHora);
        await _organizacaoRepositorio.AdicionarAsync(organizacao, CancellationToken.None);
        var contextoUsuario = new ContextoUsuario(Guid.NewGuid(), organizacao.Id);
        var handler = CriarHandler(contextoUsuario);

        var resultado = await handler.ExecutarAsync(CancellationToken.None);

        Assert.NotNull(resultado);
        Assert.Equal(organizacao.Id, resultado.OrganizacaoId);
        Assert.Equal("Clínica Bem-Estar", resultado.Nome);
        Assert.Null(resultado.EnderecoPrestador);
        Assert.Null(resultado.FusoHorario);
        Assert.False(resultado.PossuiDisponibilidade);
        Assert.False(resultado.PossuiServico);
    }

    [Fact]
    public async Task ExecutarAsync_com_perfil_operacional_configurado_retorna_endereco_e_fuso_horario()
    {
        var organizacao = Organizacao.Criar(new NomeOrganizacao("Clínica Bem-Estar"), _provedorDataHora);
        organizacao.ConfigurarPerfilOperacional(
            new EnderecoAtendimento("Rua das Flores, 123"), new FusoHorario("America/Sao_Paulo"), _provedorDataHora);
        await _organizacaoRepositorio.AdicionarAsync(organizacao, CancellationToken.None);
        var contextoUsuario = new ContextoUsuario(Guid.NewGuid(), organizacao.Id);
        var handler = CriarHandler(contextoUsuario);

        var resultado = await handler.ExecutarAsync(CancellationToken.None);

        Assert.NotNull(resultado);
        Assert.Equal("Rua das Flores, 123", resultado.EnderecoPrestador);
        Assert.Equal("America/Sao_Paulo", resultado.FusoHorario);
    }

    [Fact]
    public async Task ExecutarAsync_sem_profissional_retorna_progresso_zerado()
    {
        var organizacao = Organizacao.Criar(new NomeOrganizacao("Clínica Bem-Estar"), _provedorDataHora);
        await _organizacaoRepositorio.AdicionarAsync(organizacao, CancellationToken.None);
        var contextoUsuario = new ContextoUsuario(Guid.NewGuid(), organizacao.Id);
        var handler = CriarHandler(contextoUsuario);

        var resultado = await handler.ExecutarAsync(CancellationToken.None);

        Assert.NotNull(resultado);
        Assert.False(resultado.PossuiDisponibilidade);
        Assert.False(resultado.PossuiServico);
    }

    [Fact]
    public async Task ExecutarAsync_com_disponibilidade_e_servico_cadastrados_retorna_progresso_completo()
    {
        var organizacao = Organizacao.Criar(new NomeOrganizacao("Clínica Bem-Estar"), _provedorDataHora);
        await _organizacaoRepositorio.AdicionarAsync(organizacao, CancellationToken.None);
        var profissional = Profissional.Criar(organizacao.Id, new Nome("Dra. Ana Souza"), _provedorDataHora);
        await _profissionalRepositorio.AdicionarAsync(profissional, CancellationToken.None);

        var disponibilidade = DisponibilidadeSemanal.Criar(
            organizacao.Id, profissional.Id, DayOfWeek.Monday,
            new JanelaHorario(new TimeOnly(9, 0), new TimeOnly(18, 0)), _provedorDataHora);
        await _disponibilidadeRepositorio.AdicionarAsync(disponibilidade, CancellationToken.None);

        var servico = Servico.Criar(
            organizacao.Id, profissional.Id, new NomeServico("Consulta inicial"),
            new DuracaoServico(TimeSpan.FromMinutes(50)), new PrecoServico(250m), TipoAtendimento.Online, _provedorDataHora);
        await _servicoRepositorio.AdicionarAsync(servico, CancellationToken.None);

        var contextoUsuario = new ContextoUsuario(Guid.NewGuid(), organizacao.Id);
        var handler = CriarHandler(contextoUsuario);

        var resultado = await handler.ExecutarAsync(CancellationToken.None);

        Assert.NotNull(resultado);
        Assert.True(resultado.PossuiDisponibilidade);
        Assert.True(resultado.PossuiServico);
    }
}
