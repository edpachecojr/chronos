using Chronos.Agenda.Domain.Agendamentos.Entidades;
using Chronos.Agenda.Domain.Agendamentos.Enums;
using Chronos.Agenda.Domain.Agendamentos.Exceptions;
using Chronos.Agenda.Domain.Agendamentos.ObjetosValor;
using Chronos.Agenda.Domain.Servicos.Enums;
using Chronos.Agenda.Domain.Servicos.ObjetosValor;

namespace Chronos.Agenda.Domain.Tests.Agendamentos;

public sealed class AgendamentoTests
{
    private static readonly DateTime AgoraUtc = new(2026, 7, 21, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void ConflitaCom_QuandoIntervalosDoMesmoProfissionalSobrepoem_RetornaVerdadeiro()
    {
        var profissionalId = Guid.NewGuid();
        var primeiro = CriarAgendamento(profissionalId, AgoraUtc);
        var segundo = CriarAgendamento(profissionalId, AgoraUtc.AddMinutes(30));

        var conflita = primeiro.ConflitaCom(segundo);

        Assert.True(conflita);
    }

    [Fact]
    public void ConflitaCom_QuandoIntervalosApenasEncostam_RetornaFalso()
    {
        var profissionalId = Guid.NewGuid();
        var primeiro = CriarAgendamento(profissionalId, AgoraUtc);
        var segundo = CriarAgendamento(profissionalId, AgoraUtc.AddHours(1));

        var conflita = primeiro.ConflitaCom(segundo);

        Assert.False(conflita);
    }

    [Fact]
    public void ConflitaCom_QuandoProfissionaisSaoDiferentes_RetornaFalso()
    {
        var primeiro = CriarAgendamento(Guid.NewGuid(), AgoraUtc);
        var segundo = CriarAgendamento(Guid.NewGuid(), AgoraUtc.AddMinutes(30));

        var conflita = primeiro.ConflitaCom(segundo);

        Assert.False(conflita);
    }

    [Fact]
    public void Confirmar_QuandoPendente_AlteraStatusEDataDeAtualizacao()
    {
        var agendamento = CriarAgendamento(Guid.NewGuid(), AgoraUtc);
        var atualizadoEmUtc = AgoraUtc.AddMinutes(1);

        agendamento.Confirmar(atualizadoEmUtc);

        Assert.Equal(StatusAgendamento.Confirmado, agendamento.Status);
        Assert.Equal(atualizadoEmUtc, agendamento.AtualizadoEmUtc);
    }

    [Fact]
    public void Cancelar_QuandoJaCancelado_LancaExcecaoEspecifica()
    {
        var agendamento = CriarAgendamento(Guid.NewGuid(), AgoraUtc);
        agendamento.Cancelar(AgoraUtc.AddMinutes(1));

        var excecao = Assert.Throws<AgendamentoCanceladoException>(() => agendamento.Cancelar(AgoraUtc.AddMinutes(2)));

        Assert.Equal("Um agendamento cancelado não pode ser alterado.", excecao.Message);
    }

    [Fact]
    public void Atualizar_QuandoCancelado_LancaExcecaoEspecifica()
    {
        var agendamento = CriarAgendamento(Guid.NewGuid(), AgoraUtc);
        agendamento.Cancelar(AgoraUtc.AddMinutes(1));

        Assert.Throws<AgendamentoCanceladoException>(() => agendamento.Atualizar(
            new NomeCliente("Ana Souza"),
            CriarPeriodo(AgoraUtc.AddHours(2)),
            new PrecoServico(80m),
            TipoAtendimento.Online,
            AgoraUtc.AddMinutes(2)));
    }

    private static Agendamento CriarAgendamento(Guid profissionalId, DateTime inicioUtc)
    {
        return Agendamento.Criar(
            Guid.NewGuid(),
            profissionalId,
            Guid.NewGuid(),
            new NomeCliente("Ana Souza"),
            CriarPeriodo(inicioUtc),
            new PrecoServico(75m),
            TipoAtendimento.Presencial,
            AgoraUtc);
    }

    private static PeriodoAgendamento CriarPeriodo(DateTime inicioUtc)
    {
        return new PeriodoAgendamento(inicioUtc, new DuracaoServico(TimeSpan.FromHours(1)));
    }
}
