using Chronos.Agenda.Domain.Compartilhado.Resultados;

namespace Chronos.Agenda.Api.Erros;

/// <summary>Converte falhas do Result Pattern em respostas HTTP (<c>ProblemDetails</c>), traduzindo cada código de
/// erro esperado da Aplicação/Domínio para o status HTTP correspondente, sem vazar detalhes internos (ADR 0001).
/// Exceções de domínio não tratadas por este caminho são cenários inesperados, cobertos por
/// <see cref="ExceptionHandling.TratadorGlobalDeExcecoes"/>.</summary>
public static class ResultadoHttpExtensions
{
    /// <example><code>return resultado.ParaHttpResultado();</code></example>
    public static IResult ParaHttpResultado(this Resultado resultado, Func<IResult>? aoSucesso = null)
    {
        return resultado.Sucesso
            ? (aoSucesso ?? Results.NoContent)()
            : ParaProblema(resultado.Erro!);
    }

    /// <example><code>return resultado.ParaHttpResultado(valor => Results.Created($"/v1/servicos/{valor.ServicoId}", valor));</code></example>
    public static IResult ParaHttpResultado<T>(this Resultado<T> resultado, Func<T, IResult> aoSucesso)
    {
        return resultado.Sucesso
            ? aoSucesso(resultado.Valor)
            : ParaProblema(resultado.Erro!);
    }

    private static IResult ParaProblema(Erro erro)
    {
        return Results.Problem(
            detail: erro.Mensagem,
            statusCode: ResolverStatusHttp(erro.Codigo),
            extensions: new Dictionary<string, object?> { ["codigo"] = erro.Codigo });
    }

    private static int ResolverStatusHttp(string codigo) => codigo switch
    {
        "Autenticacao.CredenciaisInvalidas" => StatusCodes.Status401Unauthorized,

        "Organizacao.NaoEncontrada"
            or "Profissional.NaoEncontrado"
            or "Servico.NaoEncontrado"
            or "Disponibilidade.NaoEncontrada"
            or "Agendamento.NaoEncontrado" => StatusCodes.Status404NotFound,

        "Disponibilidade.JanelaSobreposta"
            or "Agendamento.ConflitoDeAgenda"
            or "Agendamento.PerfilOperacionalNaoConfigurado"
            or "Autenticacao.EmailJaCadastrado" => StatusCodes.Status409Conflict,

        _ => StatusCodes.Status400BadRequest,
    };
}
