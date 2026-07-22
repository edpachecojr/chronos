using Chronos.Agenda.Domain.Disponibilidades.Entidades;
using Chronos.Agenda.Domain.Disponibilidades.Exceptions;
using Chronos.Agenda.Domain.Disponibilidades.ObjetosValor;
using Chronos.Agenda.Domain.Tests.Compartilhado;

namespace Chronos.Agenda.Domain.Tests.Disponibilidades;

public sealed class DisponibilidadeSemanalTests
{
    [Fact]
    public void Reagendar_AtualizaDiaJanelaEAuditoria()
    {
        var criadoEmUtc = new DateTime(2026, 7, 21, 12, 0, 0, DateTimeKind.Utc);
        var provedorDataHora = new FakeProvedorDataHora(criadoEmUtc);
        var disponibilidade = DisponibilidadeSemanal.Criar(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DayOfWeek.Monday,
            new JanelaHorario(new TimeOnly(9, 0), new TimeOnly(12, 0)),
            provedorDataHora);
        provedorDataHora.UtcNow = criadoEmUtc.AddMinutes(1);
        var janela = new JanelaHorario(new TimeOnly(14, 0), new TimeOnly(18, 0));

        disponibilidade.Reagendar(DayOfWeek.Wednesday, janela, provedorDataHora);

        Assert.Equal(DayOfWeek.Wednesday, disponibilidade.DiaDaSemana);
        Assert.Equal(janela, disponibilidade.Janela);
        Assert.Equal(provedorDataHora.UtcNow, disponibilidade.Auditoria.AtualizadoEmUtc);
    }

    [Fact]
    public void JanelaHorario_QuandoFimNaoEPosteriorAoInicio_LancaExcecaoEspecifica()
    {
        Assert.Throws<JanelaHorarioInvalidaException>(
            () => new JanelaHorario(new TimeOnly(9, 0), new TimeOnly(9, 0)));
    }

    [Fact]
    public void ConflitaCom_QuandoMesmoProfissionalDiaEJanelaSobrepoem_RetornaVerdadeiro()
    {
        var profissionalId = Guid.NewGuid();
        var primeira = CriarDisponibilidade(profissionalId, DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(12, 0));
        var segunda = CriarDisponibilidade(profissionalId, DayOfWeek.Monday, new TimeOnly(11, 0), new TimeOnly(14, 0));

        Assert.True(primeira.ConflitaCom(segunda));
    }

    [Fact]
    public void ConflitaCom_QuandoJanelasApenasEncostam_RetornaFalso()
    {
        var profissionalId = Guid.NewGuid();
        var primeira = CriarDisponibilidade(profissionalId, DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(12, 0));
        var segunda = CriarDisponibilidade(profissionalId, DayOfWeek.Monday, new TimeOnly(12, 0), new TimeOnly(14, 0));

        Assert.False(primeira.ConflitaCom(segunda));
    }

    [Fact]
    public void ConflitaCom_QuandoDiasDiferentes_RetornaFalso()
    {
        var profissionalId = Guid.NewGuid();
        var primeira = CriarDisponibilidade(profissionalId, DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(12, 0));
        var segunda = CriarDisponibilidade(profissionalId, DayOfWeek.Tuesday, new TimeOnly(9, 0), new TimeOnly(12, 0));

        Assert.False(primeira.ConflitaCom(segunda));
    }

    [Fact]
    public void ConflitaCom_QuandoProfissionaisDiferentes_RetornaFalso()
    {
        var primeira = CriarDisponibilidade(Guid.NewGuid(), DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(12, 0));
        var segunda = CriarDisponibilidade(Guid.NewGuid(), DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(12, 0));

        Assert.False(primeira.ConflitaCom(segunda));
    }

    [Fact]
    public void JanelaHorario_Sobrepoe_QuandoIntervalosSeCruzam_RetornaVerdadeiro()
    {
        var janela = new JanelaHorario(new TimeOnly(9, 0), new TimeOnly(12, 0));
        var outra = new JanelaHorario(new TimeOnly(11, 0), new TimeOnly(14, 0));

        Assert.True(janela.Sobrepoe(outra));
    }

    private static DisponibilidadeSemanal CriarDisponibilidade(Guid profissionalId, DayOfWeek diaDaSemana, TimeOnly inicio, TimeOnly fim)
    {
        var provedorDataHora = new FakeProvedorDataHora(new DateTime(2026, 7, 21, 12, 0, 0, DateTimeKind.Utc));
        return DisponibilidadeSemanal.Criar(
            Guid.NewGuid(),
            profissionalId,
            diaDaSemana,
            new JanelaHorario(inicio, fim),
            provedorDataHora);
    }
}
