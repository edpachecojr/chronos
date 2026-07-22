using Chronos.Agenda.Domain.Compartilhado.Resultados;

namespace Chronos.Agenda.Domain.Tests.Compartilhado;

public class ResultadoTests
{
    private static readonly Erro ErroExemplo = new("Exemplo.Codigo", "Mensagem de exemplo.");

    [Fact]
    public void Ok_produz_resultado_bem_sucedido_com_o_valor()
    {
        var resultado = Resultado<Guid>.Ok(Guid.Empty);

        Assert.True(resultado.Sucesso);
        Assert.False(resultado.Falhou);
        Assert.Null(resultado.Erro);
        Assert.Equal(Guid.Empty, resultado.Valor);
    }

    [Fact]
    public void Falha_produz_resultado_com_o_erro_informado()
    {
        var resultado = Resultado<Guid>.Falha(ErroExemplo);

        Assert.False(resultado.Sucesso);
        Assert.True(resultado.Falhou);
        Assert.Equal(ErroExemplo, resultado.Erro);
    }

    [Fact]
    public void Valor_lanca_quando_o_resultado_e_uma_falha()
    {
        var resultado = Resultado<Guid>.Falha(ErroExemplo);

        Assert.Throws<InvalidOperationException>(() => resultado.Valor);
    }
}
