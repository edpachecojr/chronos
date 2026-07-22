using Chronos.Agenda.Application.Compartilhado.Contratos;
using Chronos.Agenda.Application.Disponibilidades.Contratos;
using Chronos.Agenda.Application.Disponibilidades.Erros;
using Chronos.Agenda.Application.Profissionais.Contratos;
using Chronos.Agenda.Application.Profissionais.Erros;
using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Resultados;
using Chronos.Agenda.Domain.Disponibilidades.Entidades;
using Chronos.Agenda.Domain.Disponibilidades.ObjetosValor;

namespace Chronos.Agenda.Application.Disponibilidades.AlterarDisponibilidade;

/// <summary>
/// Altera o dia ou a janela de uma disponibilidade semanal já configurada
/// (UC02), rejeitando sobreposição com as demais janelas do mesmo
/// profissional (Fase B item 8).
/// </summary>
/// <example><code>
/// var resultado = await handler.ExecutarAsync(
///     new AlterarDisponibilidadeComando(profissionalId, disponibilidadeId, DayOfWeek.Tuesday, new TimeOnly(14, 0), new TimeOnly(18, 0)),
///     cancellationToken);
/// </code></example>
public sealed class AlterarDisponibilidadeHandler(
    IDisponibilidadeSemanalRepositorio disponibilidadeRepositorio,
    IProfissionalRepositorio profissionalRepositorio,
    IContextoUsuario contextoUsuario,
    IUnidadeDeTrabalho unidadeDeTrabalho,
    IProvedorDataHora provedorDataHora)
{
    public async Task<Resultado> ExecutarAsync(AlterarDisponibilidadeComando comando, CancellationToken cancellationToken)
    {
        var organizacaoId = contextoUsuario.ObterOrganizacaoId();

        var profissional = await profissionalRepositorio.BuscarPorIdAsync(organizacaoId, comando.ProfissionalId, cancellationToken);
        if (profissional is null)
        {
            return Resultado.Falha(ProfissionalErros.NaoEncontrado(organizacaoId, comando.ProfissionalId));
        }

        var todasDoProfissional = await disponibilidadeRepositorio.BuscarPorProfissionalAsync(organizacaoId, profissional.Id, cancellationToken);
        var alvo = todasDoProfissional.SingleOrDefault(d => d.Id == comando.DisponibilidadeId);
        if (alvo is null)
        {
            return Resultado.Falha(DisponibilidadeErros.NaoEncontrada(comando.DisponibilidadeId));
        }

        var janelaResultado = JanelaHorarioFactory.Criar(comando.Inicio, comando.Fim);
        if (janelaResultado.Falhou)
        {
            return Resultado.Falha(janelaResultado.Erro!);
        }

        var conflito = VerificarConflito(todasDoProfissional, alvo, comando.DiaDaSemana, janelaResultado.Valor);
        if (conflito.Falhou)
        {
            return conflito;
        }

        alvo.Reagendar(comando.DiaDaSemana, janelaResultado.Valor, provedorDataHora);
        await disponibilidadeRepositorio.AtualizarAsync(alvo, cancellationToken);
        await unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);
        return Resultado.Ok();
    }

    private static Resultado VerificarConflito(
        IReadOnlyCollection<DisponibilidadeSemanal> todasDoProfissional, DisponibilidadeSemanal alvo, DayOfWeek diaDaSemana, JanelaHorario janela)
    {
        var outras = todasDoProfissional.Where(d => d.Id != alvo.Id);
        return outras.Any(d => d.DiaDaSemana == diaDaSemana && d.Janela.Sobrepoe(janela))
            ? Resultado.Falha(DisponibilidadeErros.JanelaSobreposta)
            : Resultado.Ok();
    }
}
