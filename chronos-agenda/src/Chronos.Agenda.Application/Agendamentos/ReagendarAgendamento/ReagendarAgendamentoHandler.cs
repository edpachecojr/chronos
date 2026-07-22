using Chronos.Agenda.Application.Agendamentos;
using Chronos.Agenda.Application.Agendamentos.Contratos;
using Chronos.Agenda.Application.Agendamentos.Erros;
using Chronos.Agenda.Application.Compartilhado.Contratos;
using Chronos.Agenda.Application.Disponibilidades.Contratos;
using Chronos.Agenda.Application.Organizacoes.Contratos;
using Chronos.Agenda.Application.Profissionais.Contratos;
using Chronos.Agenda.Application.Servicos.Contratos;
using Chronos.Agenda.Domain.Agendamentos.Entidades;
using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Resultados;

namespace Chronos.Agenda.Application.Agendamentos.ReagendarAgendamento;

/// <summary>
/// Reagenda ou edita um agendamento existente (UC05): reaproveita a mesma preparação e validação de UC04
/// (referências, período, local, disponibilidade RN07 e conflito RN02), excluindo o próprio agendamento da busca
/// de conflito. A transição de um agendamento já cancelado é rejeitada pelo próprio domínio
/// (<c>Agendamento.Atualizar</c>), tratada aqui como falha esperada.
/// </summary>
/// <example><code>
/// var resultado = await handler.ExecutarAsync(
///     new ReagendarAgendamentoComando(agendamentoId, profissionalId, servicoId, "Maria Silva", TipoPessoaAtendida.Paciente, novoInicio, null),
///     cancellationToken);
/// </code></example>
public sealed class ReagendarAgendamentoHandler(
    IAgendamentoRepositorio agendamentoRepositorio,
    IProfissionalRepositorio profissionalRepositorio,
    IServicoRepositorio servicoRepositorio,
    IOrganizacaoRepositorio organizacaoRepositorio,
    IDisponibilidadeSemanalRepositorio disponibilidadeRepositorio,
    IContextoUsuario contextoUsuario,
    IUnidadeDeTrabalho unidadeDeTrabalho,
    IProvedorDataHora provedorDataHora)
{
    private readonly AgendamentoPreparador _preparador = new(
        profissionalRepositorio, servicoRepositorio, organizacaoRepositorio, disponibilidadeRepositorio, agendamentoRepositorio);

    public async Task<Resultado> ExecutarAsync(ReagendarAgendamentoComando comando, CancellationToken cancellationToken)
    {
        var organizacaoId = contextoUsuario.ObterOrganizacaoId();

        var agendamentoResultado = await BuscarEValidarAgendamentoAsync(organizacaoId, comando, cancellationToken);
        if (agendamentoResultado.Falhou)
        {
            return Resultado.Falha(agendamentoResultado.Erro!);
        }

        return await PrepararEAtualizarAsync(organizacaoId, agendamentoResultado.Valor, comando, cancellationToken);
    }

    private async Task<Resultado<Agendamento>> BuscarEValidarAgendamentoAsync(
        Guid organizacaoId, ReagendarAgendamentoComando comando, CancellationToken cancellationToken)
    {
        var agendamento = await agendamentoRepositorio.BuscarPorIdAsync(organizacaoId, comando.AgendamentoId, cancellationToken);
        if (agendamento is null)
        {
            return Resultado<Agendamento>.Falha(AgendamentoErros.NaoEncontrado(organizacaoId, comando.AgendamentoId));
        }

        if (agendamento.ProfissionalId != comando.ProfissionalId || agendamento.ServicoId != comando.ServicoId)
        {
            return Resultado<Agendamento>.Falha(AgendamentoErros.AlteracaoDeProfissionalOuServicoNaoPermitida);
        }

        return Resultado<Agendamento>.Ok(agendamento);
    }

    private async Task<Resultado> PrepararEAtualizarAsync(
        Guid organizacaoId, Agendamento agendamento, ReagendarAgendamentoComando comando, CancellationToken cancellationToken)
    {
        var preparadoResultado = await _preparador.PrepararAsync(organizacaoId, comando, cancellationToken);
        if (preparadoResultado.Falhou)
        {
            return Resultado.Falha(preparadoResultado.Erro!);
        }

        var preparado = preparadoResultado.Valor;
        var regrasResultado = await _preparador.ValidarRegrasAsync(organizacaoId, preparado, comando.AgendamentoId, cancellationToken);
        if (regrasResultado.Falhou)
        {
            return regrasResultado;
        }

        var atualizarResultado = agendamento.Atualizar(preparado.PessoaAtendida, preparado.Periodo, preparado.Servico.Preco, preparado.Local, provedorDataHora);
        if (atualizarResultado.Falhou)
        {
            return atualizarResultado;
        }

        await agendamentoRepositorio.AtualizarAsync(agendamento, cancellationToken);
        await unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);
        return Resultado.Ok();
    }
}
