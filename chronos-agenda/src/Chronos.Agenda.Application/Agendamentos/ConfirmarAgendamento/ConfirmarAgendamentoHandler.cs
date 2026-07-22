using Chronos.Agenda.Application.Agendamentos.Contratos;
using Chronos.Agenda.Application.Agendamentos.Erros;
using Chronos.Agenda.Application.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Resultados;

namespace Chronos.Agenda.Application.Agendamentos.ConfirmarAgendamento;

/// <summary>
/// Confirma um agendamento pendente (UC06). A transição inválida (agendamento já confirmado ou cancelado) é
/// rejeitada pelo próprio domínio (<c>Agendamento.Confirmar</c>), tratada aqui como falha esperada.
/// </summary>
/// <example><code>
/// var resultado = await handler.ExecutarAsync(new ConfirmarAgendamentoComando(agendamentoId), cancellationToken);
/// </code></example>
public sealed class ConfirmarAgendamentoHandler(
    IAgendamentoRepositorio agendamentoRepositorio,
    IContextoUsuario contextoUsuario,
    IUnidadeDeTrabalho unidadeDeTrabalho,
    IProvedorDataHora provedorDataHora)
{
    public async Task<Resultado> ExecutarAsync(ConfirmarAgendamentoComando comando, CancellationToken cancellationToken)
    {
        var organizacaoId = contextoUsuario.ObterOrganizacaoId();

        var agendamento = await agendamentoRepositorio.BuscarPorIdAsync(organizacaoId, comando.AgendamentoId, cancellationToken);
        if (agendamento is null)
        {
            return Resultado.Falha(AgendamentoErros.NaoEncontrado(organizacaoId, comando.AgendamentoId));
        }

        var confirmarResultado = agendamento.Confirmar(provedorDataHora);
        if (confirmarResultado.Falhou)
        {
            return confirmarResultado;
        }

        await agendamentoRepositorio.AtualizarAsync(agendamento, cancellationToken);
        await unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);
        return Resultado.Ok();
    }
}
