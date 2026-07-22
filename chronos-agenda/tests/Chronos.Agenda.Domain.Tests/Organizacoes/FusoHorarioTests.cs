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
}
