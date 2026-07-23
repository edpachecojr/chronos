using Chronos.Agenda.Api.Erros;
using Chronos.Agenda.Domain.Compartilhado.Resultados;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.Agenda.Api.Tests.Erros;

public class ResultadoHttpExtensionsTests
{
    [Fact]
    public void ParaHttpResultado_sem_valor_e_bem_sucedido_sem_handler_produz_no_content()
    {
        var resultado = Resultado.Ok();

        var httpResultado = resultado.ParaHttpResultado();

        Assert.Equal(StatusCodes.Status204NoContent, ObterStatus(httpResultado));
    }

    [Fact]
    public void ParaHttpResultado_sem_valor_e_bem_sucedido_usa_o_handler_informado()
    {
        var resultado = Resultado.Ok();

        var httpResultado = resultado.ParaHttpResultado(() => Results.Ok());

        Assert.Equal(StatusCodes.Status200OK, ObterStatus(httpResultado));
    }

    [Fact]
    public void ParaHttpResultado_com_valor_e_bem_sucedido_usa_o_handler_informado()
    {
        var resultado = Resultado<Guid>.Ok(Guid.NewGuid());

        var httpResultado = resultado.ParaHttpResultado(valor => Results.Created($"/api/exemplo/{valor}", valor));

        Assert.Equal(StatusCodes.Status201Created, ObterStatus(httpResultado));
    }

    [Theory]
    [InlineData("Servico.NaoEncontrado", StatusCodes.Status404NotFound)]
    [InlineData("Disponibilidade.JanelaSobreposta", StatusCodes.Status409Conflict)]
    [InlineData("Autenticacao.CredenciaisInvalidas", StatusCodes.Status401Unauthorized)]
    [InlineData("Servico.PrecoInvalido", StatusCodes.Status400BadRequest)]
    public void ParaHttpResultado_com_falha_traduz_o_codigo_do_erro_para_o_status_http_esperado(string codigo, int statusEsperado)
    {
        var resultado = Resultado.Falha(new Erro(codigo, "Mensagem de exemplo."));

        var httpResultado = resultado.ParaHttpResultado();

        Assert.Equal(statusEsperado, ObterStatus(httpResultado));
    }

    [Fact]
    public void ParaHttpResultado_com_falha_preserva_a_mensagem_e_o_codigo_do_erro_sem_vazar_detalhes_internos()
    {
        var erro = new Erro("Servico.NaoEncontrado", "Nenhum serviço foi encontrado.");
        var resultado = Resultado<Guid>.Falha(erro);

        var httpResultado = resultado.ParaHttpResultado(valor => Results.Ok(valor));

        var problemDetails = Assert.IsAssignableFrom<IValueHttpResult<ProblemDetails>>(httpResultado).Value;
        Assert.Equal(erro.Mensagem, problemDetails!.Detail);
        Assert.Equal(erro.Codigo, problemDetails.Extensions["codigo"]);
    }

    private static int ObterStatus(IResult httpResultado)
    {
        return Assert.IsAssignableFrom<IStatusCodeHttpResult>(httpResultado).StatusCode!.Value;
    }
}
