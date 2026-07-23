namespace Chronos.Agenda.Application.Servicos.ListarServicos;

/// <summary>Filtro de leitura para listar os serviços do catálogo de um profissional (UC03).</summary>
/// <example><code>var consulta = new ListarServicosConsulta(profissionalId);</code></example>
public sealed record ListarServicosConsulta(Guid ProfissionalId);
