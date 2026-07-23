using Chronos.Agenda.Application.Servicos.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Contratos;

namespace Chronos.Agenda.Application.Servicos.ListarServicos;

/// <summary>
/// Lista os serviços do catálogo de um profissional da organização corrente (UC03). Leitura pura, sem falha
/// esperada: um profissional inexistente ou de outra organização simplesmente não tem serviços a listar.
/// </summary>
/// <example><code>var servicos = await handler.ExecutarAsync(new ListarServicosConsulta(profissionalId), cancellationToken);</code></example>
public sealed class ListarServicosHandler(IServicoRepositorio servicoRepositorio, IContextoUsuario contextoUsuario)
{
    public async Task<IReadOnlyCollection<ServicoResumo>> ExecutarAsync(ListarServicosConsulta consulta, CancellationToken cancellationToken)
    {
        var organizacaoId = contextoUsuario.ObterOrganizacaoId();
        var servicos = await servicoRepositorio.BuscarPorProfissionalAsync(organizacaoId, consulta.ProfissionalId, cancellationToken);

        return servicos
            .Select(servico => new ServicoResumo(servico.Id, servico.Nome.Valor, servico.Duracao.Valor, servico.Preco.Valor, servico.TipoAtendimento))
            .OrderBy(resumo => resumo.Nome)
            .ToList();
    }
}
