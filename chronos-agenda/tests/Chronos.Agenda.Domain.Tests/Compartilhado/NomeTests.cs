using Chronos.Agenda.Domain.Compartilhado.Exceptions;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;

namespace Chronos.Agenda.Domain.Tests.Compartilhado;

public sealed class NomeTests
{
    [Fact]
    public void Criar_QuandoValido_NormalizaEspacosNasExtremidades()
    {
        var nome = new Nome("  Marina Costa  ");

        Assert.Equal("Marina Costa", nome.Valor);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_QuandoAusente_LancaExcecaoEspecifica(string valor)
    {
        Assert.Throws<NomeInvalidoException>(() => new Nome(valor));
    }

    [Fact]
    public void Criar_QuandoExcedeComprimentoMaximo_LancaExcecaoEspecifica()
    {
        var valorMuitoLongo = new string('A', 121);

        var excecao = Assert.Throws<NomeInvalidoException>(() => new Nome(valorMuitoLongo));

        Assert.Equal("O nome deve ter entre 1 e 120 caracteres; comprimento recebido: 121.", excecao.Message);
    }

    [Fact]
    public void Criar_QuandoNoLimiteMaximo_Aceita()
    {
        var valorNoLimite = new string('A', 120);

        var nome = new Nome(valorNoLimite);

        Assert.Equal(120, nome.Valor.Length);
    }
}
