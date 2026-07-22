using Chronos.Agenda.Application.Compartilhado.Contratos;
using Chronos.Agenda.Application.Disponibilidades.Contratos;
using Chronos.Agenda.Application.Disponibilidades.Erros;
using Chronos.Agenda.Application.Profissionais.Contratos;
using Chronos.Agenda.Application.Profissionais.Erros;
using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Resultados;
using Chronos.Agenda.Domain.Disponibilidades.Entidades;

namespace Chronos.Agenda.Application.Disponibilidades.CriarDisponibilidade;

/// <summary>
/// Configura uma nova janela semanal de atendimento de um profissional (UC02),
/// rejeitando sobreposição com janelas já existentes do mesmo profissional no
/// mesmo dia (Fase B item 8).
/// </summary>
/// <example><code>
/// var resultado = await handler.ExecutarAsync(
///     new CriarDisponibilidadeComando(profissionalId, DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(12, 0)),
///     cancellationToken);
/// </code></example>
public sealed class CriarDisponibilidadeHandler(
    IDisponibilidadeSemanalRepositorio disponibilidadeRepositorio,
    IProfissionalRepositorio profissionalRepositorio,
    IContextoUsuario contextoUsuario,
    IUnidadeDeTrabalho unidadeDeTrabalho,
    IProvedorDataHora provedorDataHora)
{
    public async Task<Resultado<CriarDisponibilidadeResultado>> ExecutarAsync(
        CriarDisponibilidadeComando comando, CancellationToken cancellationToken)
    {
        var organizacaoId = contextoUsuario.ObterOrganizacaoId();

        var profissional = await profissionalRepositorio.BuscarPorIdAsync(organizacaoId, comando.ProfissionalId, cancellationToken);
        if (profissional is null)
        {
            return Resultado<CriarDisponibilidadeResultado>.Falha(ProfissionalErros.NaoEncontrado(organizacaoId, comando.ProfissionalId));
        }

        var janelaResultado = JanelaHorarioFactory.Criar(comando.Inicio, comando.Fim);
        if (janelaResultado.Falhou)
        {
            return Resultado<CriarDisponibilidadeResultado>.Falha(janelaResultado.Erro!);
        }

        var nova = DisponibilidadeSemanal.Criar(organizacaoId, profissional.Id, comando.DiaDaSemana, janelaResultado.Valor, provedorDataHora);
        var conflitoResultado = await VerificarConflitoAsync(organizacaoId, nova, cancellationToken);
        if (conflitoResultado.Falhou)
        {
            return Resultado<CriarDisponibilidadeResultado>.Falha(conflitoResultado.Erro!);
        }

        await disponibilidadeRepositorio.AdicionarAsync(nova, cancellationToken);
        await unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);
        return Resultado<CriarDisponibilidadeResultado>.Ok(new CriarDisponibilidadeResultado(nova.Id));
    }

    private async Task<Resultado> VerificarConflitoAsync(Guid organizacaoId, DisponibilidadeSemanal nova, CancellationToken cancellationToken)
    {
        var doMesmoDia = await disponibilidadeRepositorio.BuscarPorProfissionalEDiaAsync(
            organizacaoId, nova.ProfissionalId, nova.DiaDaSemana, cancellationToken);
        return doMesmoDia.Any(nova.ConflitaCom)
            ? Resultado.Falha(DisponibilidadeErros.JanelaSobreposta)
            : Resultado.Ok();
    }
}
