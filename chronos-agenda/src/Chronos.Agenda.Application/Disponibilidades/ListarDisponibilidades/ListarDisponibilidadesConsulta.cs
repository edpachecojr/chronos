namespace Chronos.Agenda.Application.Disponibilidades.ListarDisponibilidades;

/// <summary>Filtro de leitura para listar as disponibilidades semanais configuradas de um profissional (UC02).</summary>
/// <example><code>var consulta = new ListarDisponibilidadesConsulta(profissionalId);</code></example>
public sealed record ListarDisponibilidadesConsulta(Guid ProfissionalId);
