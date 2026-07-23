using Chronos.Agenda.Application.Disponibilidades.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Contratos;

namespace Chronos.Agenda.Application.Disponibilidades.ListarDisponibilidades;

/// <summary>
/// Lista as disponibilidades semanais configuradas de um profissional da organização corrente (UC02). Leitura
/// pura, sem falha esperada: um profissional inexistente ou de outra organização simplesmente não tem
/// disponibilidades a listar.
/// </summary>
/// <example><code>var disponibilidades = await handler.ExecutarAsync(new ListarDisponibilidadesConsulta(profissionalId), cancellationToken);</code></example>
public sealed class ListarDisponibilidadesHandler(IDisponibilidadeSemanalRepositorio disponibilidadeRepositorio, IContextoUsuario contextoUsuario)
{
    public async Task<IReadOnlyCollection<DisponibilidadeResumo>> ExecutarAsync(ListarDisponibilidadesConsulta consulta, CancellationToken cancellationToken)
    {
        var organizacaoId = contextoUsuario.ObterOrganizacaoId();
        var disponibilidades = await disponibilidadeRepositorio.BuscarPorProfissionalAsync(organizacaoId, consulta.ProfissionalId, cancellationToken);

        return disponibilidades
            .Select(disponibilidade => new DisponibilidadeResumo(disponibilidade.Id, disponibilidade.DiaDaSemana, disponibilidade.Janela.Inicio, disponibilidade.Janela.Fim))
            .OrderBy(resumo => resumo.DiaDaSemana)
            .ThenBy(resumo => resumo.Inicio)
            .ToList();
    }
}
