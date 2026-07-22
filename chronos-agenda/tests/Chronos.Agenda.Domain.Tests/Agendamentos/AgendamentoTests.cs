using Chronos.Agenda.Domain.Agendamentos.Entidades;
using Chronos.Agenda.Domain.Agendamentos.Enums;
using Chronos.Agenda.Domain.Agendamentos.Exceptions;
using Chronos.Agenda.Domain.Agendamentos.ObjetosValor;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Servicos.Enums;
using Chronos.Agenda.Domain.Servicos.ObjetosValor;
using Chronos.Agenda.Domain.Tests.Compartilhado;

namespace Chronos.Agenda.Domain.Tests.Agendamentos;

public sealed class AgendamentoTests
{
    private static readonly DateTime AgoraUtc = new(2026, 7, 21, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Criar_PreservaPessoaPeriodoPrecoELocal()
    {
        var periodo = CriarPeriodo(AgoraUtc);
        var pessoaAtendida = new PessoaAtendida(new Nome("  Marina Silva  "), TipoPessoaAtendida.Paciente);
        var local = LocalAtendimento.Domiciliar(new EnderecoAtendimento("  Rua das Flores, 10  "));

        var agendamento = CriarAgendamento(Guid.NewGuid(), periodo, pessoaAtendida, local);

        Assert.Equal("Marina Silva", agendamento.PessoaAtendida.Nome.Valor);
        Assert.Equal(TipoPessoaAtendida.Paciente, agendamento.PessoaAtendida.Tipo);
        Assert.Equal(periodo, agendamento.Periodo);
        Assert.Equal(75m, agendamento.PrecoCobrado.Valor);
        Assert.Equal(TipoAtendimento.Domiciliar, agendamento.TipoAtendimento);
        Assert.Equal("Rua das Flores, 10", agendamento.Local.Endereco!.Descricao);
    }

    [Fact]
    public void Criar_PreservaNomeDoServicoContratadoEDuracaoReservada()
    {
        var periodo = new PeriodoAgendamento(AgoraUtc, AgoraUtc.AddMinutes(45));

        var agendamento = CriarAgendamento(Guid.NewGuid(), periodo, nomeServicoContratado: "Consulta inicial");

        Assert.Equal("Consulta inicial", agendamento.NomeServicoContratado);
        Assert.Equal(TimeSpan.FromMinutes(45), agendamento.DuracaoReservada);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_QuandoNomeDoServicoContratadoAusente_LancaArgumentException(string? nomeServicoContratado)
    {
        Assert.ThrowsAny<ArgumentException>(() => Agendamento.Criar(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            nomeServicoContratado!,
            new PessoaAtendida(new Nome("Ana Souza"), TipoPessoaAtendida.Cliente),
            CriarPeriodo(AgoraUtc),
            new PrecoServico(75m),
            LocalAtendimento.Online(),
            new FakeProvedorDataHora(AgoraUtc)));
    }

    [Fact]
    public void ConflitaCom_QuandoIntervalosDoMesmoProfissionalSobrepoem_RetornaVerdadeiro()
    {
        var profissionalId = Guid.NewGuid();
        var primeiro = CriarAgendamento(profissionalId, CriarPeriodo(AgoraUtc));
        var segundo = CriarAgendamento(profissionalId, CriarPeriodo(AgoraUtc.AddMinutes(30)));

        Assert.True(primeiro.ConflitaCom(segundo));
    }

    [Fact]
    public void ConflitaCom_QuandoIntervalosApenasEncostam_RetornaFalso()
    {
        var profissionalId = Guid.NewGuid();
        var primeiro = CriarAgendamento(profissionalId, CriarPeriodo(AgoraUtc));
        var segundo = CriarAgendamento(profissionalId, CriarPeriodo(AgoraUtc.AddHours(1)));

        Assert.False(primeiro.ConflitaCom(segundo));
    }

    [Fact]
    public void ConflitaCom_QuandoUmAgendamentoEstaCancelado_RetornaFalso()
    {
        var profissionalId = Guid.NewGuid();
        var primeiro = CriarAgendamento(profissionalId, CriarPeriodo(AgoraUtc));
        var segundo = CriarAgendamento(profissionalId, CriarPeriodo(AgoraUtc.AddMinutes(30)));
        segundo.Cancelar(new FakeProvedorDataHora(AgoraUtc.AddMinutes(1)));

        Assert.False(primeiro.ConflitaCom(segundo));
    }

    [Fact]
    public void Confirmar_QuandoPendente_AlteraStatusEDataDeAtualizacao()
    {
        var agendamento = CriarAgendamento(Guid.NewGuid(), CriarPeriodo(AgoraUtc));
        var provedorDataHora = new FakeProvedorDataHora(AgoraUtc.AddMinutes(1));

        agendamento.Confirmar(provedorDataHora);

        Assert.Equal(StatusAgendamento.Confirmado, agendamento.Status);
        Assert.Equal(provedorDataHora.UtcNow, agendamento.Auditoria.AtualizadoEmUtc);
    }

    [Fact]
    public void Cancelar_QuandoJaCancelado_LancaExcecaoEspecifica()
    {
        var agendamento = CriarAgendamento(Guid.NewGuid(), CriarPeriodo(AgoraUtc));
        var provedorDataHora = new FakeProvedorDataHora(AgoraUtc.AddMinutes(1));
        agendamento.Cancelar(provedorDataHora);

        var excecao = Assert.Throws<AgendamentoCanceladoException>(() => agendamento.Cancelar(provedorDataHora));

        Assert.Equal("Um agendamento cancelado não pode ser alterado.", excecao.Message);
    }

    [Fact]
    public void Atualizar_QuandoCancelado_LancaExcecaoEspecifica()
    {
        var agendamento = CriarAgendamento(Guid.NewGuid(), CriarPeriodo(AgoraUtc));
        var provedorDataHora = new FakeProvedorDataHora(AgoraUtc.AddMinutes(1));
        agendamento.Cancelar(provedorDataHora);

        Assert.Throws<AgendamentoCanceladoException>(() => agendamento.Atualizar(
            new PessoaAtendida(new Nome("Ana Souza"), TipoPessoaAtendida.Cliente),
            CriarPeriodo(AgoraUtc.AddHours(2)),
            new PrecoServico(80m),
            LocalAtendimento.Online(),
            provedorDataHora));
    }

    [Fact]
    public void PeriodoAgendamento_QuandoValido_CalculaDuracaoEmMinutos()
    {
        var periodo = new PeriodoAgendamento(AgoraUtc, AgoraUtc.AddMinutes(45));

        Assert.Equal(TimeSpan.FromMinutes(45), periodo.Duracao);
        Assert.Equal(45, periodo.DuracaoEmMinutos);
    }

    [Fact]
    public void PeriodoAgendamento_APartirDaDuracao_CalculaFimAPartirDaDuracaoDoServico()
    {
        var periodo = PeriodoAgendamento.APartirDaDuracao(AgoraUtc, new DuracaoServico(TimeSpan.FromMinutes(30)));

        Assert.Equal(AgoraUtc, periodo.InicioUtc);
        Assert.Equal(AgoraUtc.AddMinutes(30), periodo.FimUtc);
    }

    [Fact]
    public void PeriodoAgendamento_QuandoFimNaoEPosteriorAoInicio_LancaExcecaoEspecifica()
    {
        Assert.Throws<FimAgendamentoInvalidoException>(() => new PeriodoAgendamento(AgoraUtc, AgoraUtc));
    }

    [Fact]
    public void PeriodoAgendamento_QuandoFimNaoEstaEmUtc_LancaExcecaoEspecifica()
    {
        var fimLocal = DateTime.SpecifyKind(AgoraUtc.AddHours(1), DateTimeKind.Local);

        var excecao = Assert.Throws<DataHoraAgendamentoNaoEstaEmUtcException>(() => new PeriodoAgendamento(AgoraUtc, fimLocal));

        Assert.Contains("fimUtc", excecao.Message);
    }

    [Fact]
    public void LocalAtendimento_Online_NaoPossuiEnderecoFisico()
    {
        var local = LocalAtendimento.Online();

        Assert.Null(local.Endereco);
    }

    [Fact]
    public void LocalAtendimento_NoEnderecoDoPrestador_PreservaEndereco()
    {
        var local = LocalAtendimento.NoEnderecoDoPrestador(new EnderecoAtendimento("Av. Central, 20"));

        Assert.Equal("Av. Central, 20", local.Endereco!.Descricao);
    }

    [Fact]
    public void PessoaAtendida_QuandoNomeNulo_LancaArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new PessoaAtendida(null!, TipoPessoaAtendida.Outro));
    }

    private static Agendamento CriarAgendamento(
        Guid profissionalId,
        PeriodoAgendamento periodo,
        PessoaAtendida? pessoaAtendida = null,
        LocalAtendimento? local = null,
        string nomeServicoContratado = "Consulta")
    {
        return Agendamento.Criar(
            Guid.NewGuid(),
            profissionalId,
            Guid.NewGuid(),
            nomeServicoContratado,
            pessoaAtendida ?? new PessoaAtendida(new Nome("Ana Souza"), TipoPessoaAtendida.Cliente),
            periodo,
            new PrecoServico(75m),
            local ?? LocalAtendimento.NoEnderecoDoPrestador(new EnderecoAtendimento("Rua Exemplo, 1")),
            new FakeProvedorDataHora(AgoraUtc));
    }

    private static PeriodoAgendamento CriarPeriodo(DateTime inicioUtc)
    {
        return new PeriodoAgendamento(inicioUtc, inicioUtc.AddHours(1));
    }
}
