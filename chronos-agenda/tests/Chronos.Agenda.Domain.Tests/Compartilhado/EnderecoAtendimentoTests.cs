using Chronos.Agenda.Domain.Compartilhado.Exceptions;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;

namespace Chronos.Agenda.Domain.Tests.Compartilhado;

public sealed class EnderecoAtendimentoTests
{
    [Fact]
    public void Criar_QuandoValido_NormalizaEspacosNasExtremidades()
    {
        var endereco = new EnderecoAtendimento("  Rua das Flores, 10  ");

        Assert.Equal("Rua das Flores, 10", endereco.Descricao);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_QuandoAusente_LancaExcecaoEspecifica(string descricao)
    {
        Assert.Throws<EnderecoAtendimentoInvalidoException>(() => new EnderecoAtendimento(descricao));
    }

    [Fact]
    public void Criar_QuandoExcedeComprimentoMaximo_LancaExcecaoEspecifica()
    {
        var descricaoMuitoLonga = new string('A', 301);

        var excecao = Assert.Throws<EnderecoAtendimentoInvalidoException>(() => new EnderecoAtendimento(descricaoMuitoLonga));

        Assert.Equal("O endereço do atendimento deve ter entre 1 e 300 caracteres; comprimento recebido: 301.", excecao.Message);
    }

    [Fact]
    public void Criar_QuandoNoLimiteMaximo_Aceita()
    {
        var descricaoNoLimite = new string('A', 300);

        var endereco = new EnderecoAtendimento(descricaoNoLimite);

        Assert.Equal(300, endereco.Descricao.Length);
    }
}
