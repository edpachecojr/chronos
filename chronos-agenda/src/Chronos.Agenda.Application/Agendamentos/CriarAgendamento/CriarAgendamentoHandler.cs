using Chronos.Agenda.Application.Agendamentos;
using Chronos.Agenda.Application.Agendamentos.Contratos;
using Chronos.Agenda.Application.Compartilhado.Contratos;
using Chronos.Agenda.Application.Disponibilidades.Contratos;
using Chronos.Agenda.Application.Organizacoes.Contratos;
using Chronos.Agenda.Application.Profissionais.Contratos;
using Chronos.Agenda.Application.Servicos.Contratos;
using Chronos.Agenda.Domain.Agendamentos.Entidades;
using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Resultados;

namespace Chronos.Agenda.Application.Agendamentos.CriarAgendamento;

/// <summary>
/// Cria um novo agendamento (UC04): valida profissional, serviço e seu
/// vínculo (RN04), calcula o período pela duração vigente do serviço (RN05),
/// resolve e valida o local de atendimento (RN06), verifica se o período cabe
/// na disponibilidade do profissional (RN07) e rejeita conflito com outro
/// agendamento ativo (RN02). A preparação e a validação são compartilhadas
/// com o reagendamento (UC05) via <see cref="AgendamentoPreparador"/>.
/// </summary>
/// <example><code>
/// var resultado = await handler.ExecutarAsync(
///     new CriarAgendamentoComando(profissionalId, servicoId, "Maria Silva", TipoPessoaAtendida.Paciente, inicio, null),
///     cancellationToken);
/// </code></example>
public sealed class CriarAgendamentoHandler(
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

    public async Task<Resultado<CriarAgendamentoResultado>> ExecutarAsync(CriarAgendamentoComando comando, CancellationToken cancellationToken)
    {
        var organizacaoId = contextoUsuario.ObterOrganizacaoId();

        var preparadoResultado = await _preparador.PrepararAsync(organizacaoId, comando, cancellationToken);
        if (preparadoResultado.Falhou)
        {
            return Resultado<CriarAgendamentoResultado>.Falha(preparadoResultado.Erro!);
        }

        var preparado = preparadoResultado.Valor;
        var regrasResultado = await _preparador.ValidarRegrasAsync(organizacaoId, preparado, excluirAgendamentoId: null, cancellationToken);
        if (regrasResultado.Falhou)
        {
            return Resultado<CriarAgendamentoResultado>.Falha(regrasResultado.Erro!);
        }

        var agendamento = Agendamento.Criar(
            organizacaoId, preparado.Profissional.Id, preparado.Servico.Id, preparado.Servico.Nome.Valor,
            preparado.PessoaAtendida, preparado.Periodo, preparado.Servico.Preco, preparado.Local, provedorDataHora);
        await agendamentoRepositorio.AdicionarAsync(agendamento, cancellationToken);
        await unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);
        return Resultado<CriarAgendamentoResultado>.Ok(new CriarAgendamentoResultado(agendamento.Id));
    }
}
