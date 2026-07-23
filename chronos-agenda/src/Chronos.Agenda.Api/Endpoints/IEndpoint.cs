namespace Chronos.Agenda.Api.Endpoints;

/// <summary>Contrato para padronizar o registro de um único endpoint minimalista da Api, sem controllers (ADR 0001).
/// Cada endpoint implementa esta interface com um membro estático, para que <see cref="Endpoint.MapearEndpoints"/>
/// componha a rota chamando o tipo diretamente, sem instanciá-lo nem depender do contêiner de injeção de
/// dependências para descobri-lo.</summary>
public interface IEndpoint
{
    /// <summary>Mapeia a rota deste endpoint no grupo informado.</summary>
    /// <example><code>rotas.MapPost("/", CriarAsync);</code></example>
    static abstract void MapearEndpoint(IEndpointRouteBuilder rotas);
}
