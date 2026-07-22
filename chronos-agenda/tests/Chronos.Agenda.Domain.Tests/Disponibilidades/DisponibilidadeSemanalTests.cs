using Chronos.Agenda.Domain.Disponibilidades.Entidades;
using Chronos.Agenda.Domain.Disponibilidades.Excecoes;
using Chronos.Agenda.Domain.Disponibilidades.ObjetosValor;

namespace Chronos.Agenda.Domain.Tests.Disponibilidades;

public sealed class DisponibilidadeSemanalTests
{
    [Fact]
    public void Reagendar_AtualizaDiaJanelaEAuditoria()
    {
        var criadoEmUtc = new DateTime(2026, 7, 21, 12, 0, 0, DateTimeKind.Utc);
        var disponibilidade = DisponibilidadeSemanal.Criar(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DayOfWeek.Monday,
            new JanelaHorario(new TimeOnly(9, 0), new TimeOnly(12, 0)),
            criadoEmUtc);
        var atualizadoEmUtc = criadoEmUtc.AddMinutes(1);
        var janela = new JanelaHorario(new TimeOnly(14, 0), new TimeOnly(18, 0));

        disponibilidade.Reagendar(DayOfWeek.Wednesday, janela, atualizadoEmUtc);

        Assert.Equal(DayOfWeek.Wednesday, disponibilidade.DiaDaSemana);
        Assert.Equal(janela, disponibilidade.Janela);
        Assert.Equal(atualizadoEmUtc, disponibilidade.AtualizadoEmUtc);
    }

    [Fact]
    public void JanelaHorario_QuandoFimNaoEPosteriorAoInicio_LancaExcecaoEspecifica()
    {
        Assert.Throws<JanelaHorarioInvalidaException>(
            () => new JanelaHorario(new TimeOnly(9, 0), new TimeOnly(9, 0)));
    }
}
