using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.Agenda.Api.ExceptionHandling;

/// <summary>Converte exceções não tratadas em uma resposta HTTP genérica, sem vazar detalhes internos (ADR 0001).
/// Exceções de domínio esperadas já são convertidas em <see cref="Domain.Compartilhado.Resultados.Resultado"/>
/// pelos próprios handlers de caso de uso (ver <see cref="Erros.ResultadoHttpExtensions"/>); o que chega aqui é
/// sempre um cenário não previsto pelo fluxo de negócio.</summary>
public sealed class TratadorGlobalDeExcecoes(IProblemDetailsService problemDetailsService, ILogger<TratadorGlobalDeExcecoes> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception excecao, CancellationToken cancellationToken)
    {
        logger.LogError(excecao, "Falha não tratada ao processar {Metodo} {Caminho}", httpContext.Request.Method, httpContext.Request.Path);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = excecao,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Ocorreu uma falha inesperada ao processar a requisição.",
            },
        });
    }
}
