using Chronos.Agenda.Application.Compartilhado.Contratos;
using Chronos.Agenda.Application.Servicos.Contratos;
using Chronos.Agenda.Application.Servicos.Erros;
using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Resultados;

namespace Chronos.Agenda.Application.Servicos.AtualizarServico;

/// <summary>
/// Atualiza a configuração comercial de um serviço já existente (UC03). Não
/// reescreve agendamentos já criados: o snapshot comercial em
/// <c>Agendamento</c> isola o histórico de mudanças futuras no catálogo
/// (Fase B item 7).
/// </summary>
/// <example><code>
/// var resultado = await handler.ExecutarAsync(
///     new AtualizarServicoComando(servicoId, "Consulta de retorno", TimeSpan.FromMinutes(30), 180m, TipoAtendimento.Online),
///     cancellationToken);
/// </code></example>
public sealed class AtualizarServicoHandler(
    IServicoRepositorio servicoRepositorio,
    IContextoUsuario contextoUsuario,
    IUnidadeDeTrabalho unidadeDeTrabalho,
    IProvedorDataHora provedorDataHora)
{
    public async Task<Resultado> ExecutarAsync(AtualizarServicoComando comando, CancellationToken cancellationToken)
    {
        var organizacaoId = contextoUsuario.ObterOrganizacaoId();

        var servico = await servicoRepositorio.BuscarPorIdAsync(organizacaoId, comando.ServicoId, cancellationToken);
        if (servico is null)
        {
            return Resultado.Falha(ServicoErros.NaoEncontrado(organizacaoId, comando.ServicoId));
        }

        var configuracaoResultado = ConfiguracaoServicoFactory.Criar(comando.Nome, comando.Duracao, comando.Preco);
        if (configuracaoResultado.Falhou)
        {
            return Resultado.Falha(configuracaoResultado.Erro!);
        }

        var configuracao = configuracaoResultado.Valor;
        servico.Atualizar(configuracao.Nome, configuracao.Duracao, configuracao.Preco, comando.TipoAtendimento, provedorDataHora);
        await servicoRepositorio.AtualizarAsync(servico, cancellationToken);
        await unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);
        return Resultado.Ok();
    }
}
