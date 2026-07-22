using Chronos.Agenda.Domain.Organizacoes.Exceptions;
using Chronos.Agenda.Domain.Organizacoes.ObjetosValor;

namespace Chronos.Agenda.Domain.Tests.Organizacoes;

public sealed class FusoHorarioTests
{
    [Fact]
    public void Criar_QuandoIdentificadorIanaValido_Aceita()
    {
        var fusoHorario = new FusoHorario("America/Sao_Paulo");

        Assert.Equal("America/Sao_Paulo", fusoHorario.Identificador);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_QuandoIdentificadorAusente_LancaExcecaoEspecifica(string identificador)
    {
        Assert.Throws<FusoHorarioInvalidoException>(() => new FusoHorario(identificador));
    }

    [Fact]
    public void Criar_QuandoIdentificadorNaoReconhecido_LancaExcecaoEspecifica()
    {
        var excecao = Assert.Throws<FusoHorarioInvalidoException>(() => new FusoHorario("Regiao/Inexistente"));

        Assert.Equal(
            "O fuso horário deve ser um identificador IANA reconhecido; identificador recebido: 'Regiao/Inexistente'.",
            excecao.Message);
    }

    [Fact]
    public void ConverterParaLocal_QuandoHorarioPadrao_AplicaOffsetCorreto()
    {
        var fusoHorario = new FusoHorario("America/Sao_Paulo");
        var instanteUtc = new DateTime(2026, 7, 21, 12, 0, 0, DateTimeKind.Utc);

        var instanteLocal = fusoHorario.ConverterParaLocal(instanteUtc);

        Assert.Equal(new DateTime(2026, 7, 21, 9, 0, 0), instanteLocal.DateTime);
        Assert.Equal(TimeSpan.FromHours(-3), instanteLocal.Offset);
    }

    [Fact]
    public void ConverterParaLocal_QuandoInstanteSemMarcacaoUtc_ConverteMesmoAssim()
    {
        var fusoHorario = new FusoHorario("America/Sao_Paulo");
        var instanteUtc = new DateTime(2026, 7, 21, 12, 0, 0, DateTimeKind.Unspecified);

        var instanteLocal = fusoHorario.ConverterParaLocal(instanteUtc);

        Assert.Equal(new DateTime(2026, 7, 21, 9, 0, 0), instanteLocal.DateTime);
    }
}
