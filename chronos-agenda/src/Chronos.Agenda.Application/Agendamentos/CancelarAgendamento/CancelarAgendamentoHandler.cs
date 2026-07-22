using Chronos.Agenda.Application.Agendamentos.Contratos;
using Chronos.Agenda.Application.Agendamentos.Erros;
using Chronos.Agenda.Application.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Resultados;

namespace Chronos.Agenda.Application.Agendamentos.CancelarAgendamento;

/// <summary>
/// Cancela um agendamento ativo (UC06). Cancelar um agendamento já cancelado é rejeitado pelo próprio domínio
/// (<c>Agendamento.Cancelar</c>), tratado aqui como falha esperada.
/// </summary>
/// <example><code>
/// var resultado = await handler.ExecutarAsync(new CancelarAgendamentoComando(agendamentoId), cancellationToken);
/// </code></example>
public sealed class CancelarAgendamentoHandler(
    IAgendamentoRepositorio agendamentoRepositorio,
    IContextoUsuario contextoUsuario,
    IUnidadeDeTrabalho unidadeDeTrabalho,
    IProvedorDataHora provedorDataHora)
{
    public async Task<Resultado> ExecutarAsync(CancelarAgendamentoComando comando, CancellationToken cancellationToken)
    {
        var organizacaoId = contextoUsuario.ObterOrganizacaoId();

        var agendamento = await agendamentoRepositorio.BuscarPorIdAsync(organizacaoId, comando.AgendamentoId, cancellationToken);
        if (agendamento is null)
        {
            return Resultado.Falha(AgendamentoErros.NaoEncontrado(organizacaoId, comando.AgendamentoId));
        }

        var cancelarResultado = agendamento.Cancelar(provedorDataHora);
        if (cancelarResultado.Falhou)
        {
            return cancelarResultado;
        }

        await agendamentoRepositorio.AtualizarAsync(agendamento, cancellationToken);
        await unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);
        return Resultado.Ok();
    }
}
