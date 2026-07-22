using Chronos.Agenda.Domain.Agendamentos.ObjetosValor;
using Chronos.Agenda.Domain.Disponibilidades.Entidades;
using Chronos.Agenda.Domain.Disponibilidades.Exceptions;
using Chronos.Agenda.Domain.Disponibilidades.ObjetosValor;
using Chronos.Agenda.Domain.Disponibilidades.Servicos;
using Chronos.Agenda.Domain.Organizacoes.ObjetosValor;
using Chronos.Agenda.Domain.Tests.Compartilhado;

namespace Chronos.Agenda.Domain.Tests.Disponibilidades;

public sealed class VerificadorDisponibilidadeTests
{
    // 2026-07-21T13:00Z é terça-feira 10:00 em America/Sao_Paulo (UTC-3, sem horário de verão).
    private static readonly DateTime InicioUtc = new(2026, 7, 21, 13, 0, 0, DateTimeKind.Utc);
    private static readonly FusoHorario FusoDeSaoPaulo = new("America/Sao_Paulo");

    [Fact]
    public void Verificar_QuandoPeriodoCabeNaJanelaDoDia_NaoLancaExcecao()
    {
        var periodo = new PeriodoAgendamento(InicioUtc, InicioUtc.AddHours(1));
        var disponibilidades = new[] { CriarDisponibilidade(DayOfWeek.Tuesday, new TimeOnly(9, 0), new TimeOnly(12, 0)) };

        VerificadorDisponibilidade.Verificar(periodo, FusoDeSaoPaulo, disponibilidades);
    }

    [Fact]
    public void Verificar_QuandoPeriodoForaDoHorarioDaJanela_LancaExcecaoEspecifica()
    {
        var periodo = new PeriodoAgendamento(InicioUtc, InicioUtc.AddHours(1));
        var disponibilidades = new[] { CriarDisponibilidade(DayOfWeek.Tuesday, new TimeOnly(14, 0), new TimeOnly(18, 0)) };

        Assert.Throws<PeriodoForaDaDisponibilidadeException>(
            () => VerificadorDisponibilidade.Verificar(periodo, FusoDeSaoPaulo, disponibilidades));
    }

    [Fact]
    public void Verificar_QuandoDisponibilidadeENoutroDia_LancaExcecaoEspecifica()
    {
        var periodo = new PeriodoAgendamento(InicioUtc, InicioUtc.AddHours(1));
        var disponibilidades = new[] { CriarDisponibilidade(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(12, 0)) };

        Assert.Throws<PeriodoForaDaDisponibilidadeException>(
            () => VerificadorDisponibilidade.Verificar(periodo, FusoDeSaoPaulo, disponibilidades));
    }

    [Fact]
    public void Verificar_QuandoPeriodoUltrapassaOFimDaJanela_LancaExcecaoEspecifica()
    {
        var periodo = new PeriodoAgendamento(InicioUtc, InicioUtc.AddHours(3));
        var disponibilidades = new[] { CriarDisponibilidade(DayOfWeek.Tuesday, new TimeOnly(9, 0), new TimeOnly(11, 0)) };

        Assert.Throws<PeriodoForaDaDisponibilidadeException>(
            () => VerificadorDisponibilidade.Verificar(periodo, FusoDeSaoPaulo, disponibilidades));
    }

    private static DisponibilidadeSemanal CriarDisponibilidade(DayOfWeek diaDaSemana, TimeOnly inicio, TimeOnly fim)
    {
        var provedorDataHora = new FakeProvedorDataHora(InicioUtc);
        return DisponibilidadeSemanal.Criar(
            Guid.NewGuid(),
            Guid.NewGuid(),
            diaDaSemana,
            new JanelaHorario(inicio, fim),
            provedorDataHora);
    }
}
