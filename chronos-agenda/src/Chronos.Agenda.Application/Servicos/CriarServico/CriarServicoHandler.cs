using Chronos.Agenda.Application.Compartilhado.Contratos;
using Chronos.Agenda.Application.Profissionais.Contratos;
using Chronos.Agenda.Application.Profissionais.Erros;
using Chronos.Agenda.Application.Servicos.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Resultados;
using Chronos.Agenda.Domain.Servicos.Entidades;

namespace Chronos.Agenda.Application.Servicos.CriarServico;

/// <summary>Cria um novo serviço oferecido por um profissional da organização corrente (UC03).</summary>
/// <example><code>
/// var resultado = await handler.ExecutarAsync(
///     new CriarServicoComando(profissionalId, "Consulta inicial", TimeSpan.FromMinutes(50), 250m, TipoAtendimento.Online),
///     cancellationToken);
/// </code></example>
public sealed class CriarServicoHandler(
    IServicoRepositorio servicoRepositorio,
    IProfissionalRepositorio profissionalRepositorio,
    IContextoUsuario contextoUsuario,
    IUnidadeDeTrabalho unidadeDeTrabalho,
    IProvedorDataHora provedorDataHora)
{
    public async Task<Resultado<CriarServicoResultado>> ExecutarAsync(CriarServicoComando comando, CancellationToken cancellationToken)
    {
        var organizacaoId = contextoUsuario.ObterOrganizacaoId();

        var profissional = await profissionalRepositorio.BuscarPorIdAsync(organizacaoId, comando.ProfissionalId, cancellationToken);
        if (profissional is null)
        {
            return Resultado<CriarServicoResultado>.Falha(ProfissionalErros.NaoEncontrado(organizacaoId, comando.ProfissionalId));
        }

        var configuracaoResultado = ConfiguracaoServicoFactory.Criar(comando.Nome, comando.Duracao, comando.Preco);
        if (configuracaoResultado.Falhou)
        {
            return Resultado<CriarServicoResultado>.Falha(configuracaoResultado.Erro!);
        }

        var configuracao = configuracaoResultado.Valor;
        var servico = Servico.Criar(
            organizacaoId, profissional.Id, configuracao.Nome, configuracao.Duracao, configuracao.Preco, comando.TipoAtendimento, provedorDataHora);
        await servicoRepositorio.AdicionarAsync(servico, cancellationToken);
        await unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);
        return Resultado<CriarServicoResultado>.Ok(new CriarServicoResultado(servico.Id));
    }
}
