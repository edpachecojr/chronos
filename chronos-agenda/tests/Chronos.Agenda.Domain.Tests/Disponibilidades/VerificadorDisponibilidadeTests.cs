using Chronos.Agenda.Domain.Disponibilidades.Entidades;
using Chronos.Agenda.Domain.Disponibilidades.Erros;
using Chronos.Agenda.Domain.Disponibilidades.ObjetosValor;
using Chronos.Agenda.Domain.Disponibilidades.Servicos;
using Chronos.Agenda.Domain.Tests.Compartilhado;

namespace Chronos.Agenda.Domain.Tests.Disponibilidades;

public sealed class VerificadorDisponibilidadeTests
{
    private static readonly DateTime CriadoEmUtc = new(2026, 7, 21, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Verificar_QuandoJanelaOcupadaCabeNaDisponibilidadeDoDia_RetornaSucesso()
    {
        var janelaOcupada = new JanelaHorario(new TimeOnly(10, 0), new TimeOnly(11, 0));
        var disponibilidades = new[] { CriarDisponibilidade(DayOfWeek.Tuesday, new TimeOnly(9, 0), new TimeOnly(12, 0)) };

        var resultado = VerificadorDisponibilidade.Verificar(DayOfWeek.Tuesday, janelaOcupada, disponibilidades);

        Assert.True(resultado.Sucesso);
    }

    [Fact]
    public void Verificar_QuandoJanelaOcupadaForaDoHorarioDaDisponibilidade_RetornaFalhaComErroEspecifico()
    {
        var janelaOcupada = new JanelaHorario(new TimeOnly(10, 0), new TimeOnly(11, 0));
        var disponibilidades = new[] { CriarDisponibilidade(DayOfWeek.Tuesday, new TimeOnly(14, 0), new TimeOnly(18, 0)) };

        var resultado = VerificadorDisponibilidade.Verificar(DayOfWeek.Tuesday, janelaOcupada, disponibilidades);

        Assert.True(resultado.Falhou);
        Assert.Equal(DisponibilidadeErros.ForaDaJanela(DayOfWeek.Tuesday, janelaOcupada), resultado.Erro);
    }

    [Fact]
    public void Verificar_QuandoDisponibilidadeENoutroDia_RetornaFalhaComErroEspecifico()
    {
        var janelaOcupada = new JanelaHorario(new TimeOnly(10, 0), new TimeOnly(11, 0));
        var disponibilidades = new[] { CriarDisponibilidade(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(12, 0)) };

        var resultado = VerificadorDisponibilidade.Verificar(DayOfWeek.Tuesday, janelaOcupada, disponibilidades);

        Assert.True(resultado.Falhou);
        Assert.Equal(DisponibilidadeErros.ForaDaJanela(DayOfWeek.Tuesday, janelaOcupada), resultado.Erro);
    }

    [Fact]
    public void Verificar_QuandoJanelaOcupadaUltrapassaOFimDaDisponibilidade_RetornaFalhaComErroEspecifico()
    {
        var janelaOcupada = new JanelaHorario(new TimeOnly(10, 0), new TimeOnly(13, 0));
        var disponibilidades = new[] { CriarDisponibilidade(DayOfWeek.Tuesday, new TimeOnly(9, 0), new TimeOnly(11, 0)) };

        var resultado = VerificadorDisponibilidade.Verificar(DayOfWeek.Tuesday, janelaOcupada, disponibilidades);

        Assert.True(resultado.Falhou);
        Assert.Equal(DisponibilidadeErros.ForaDaJanela(DayOfWeek.Tuesday, janelaOcupada), resultado.Erro);
    }

    private static DisponibilidadeSemanal CriarDisponibilidade(DayOfWeek diaDaSemana, TimeOnly inicio, TimeOnly fim)
    {
        var provedorDataHora = new FakeProvedorDataHora(CriadoEmUtc);
        return DisponibilidadeSemanal.Criar(
            Guid.NewGuid(),
            Guid.NewGuid(),
            diaDaSemana,
            new JanelaHorario(inicio, fim),
            provedorDataHora);
    }
}
