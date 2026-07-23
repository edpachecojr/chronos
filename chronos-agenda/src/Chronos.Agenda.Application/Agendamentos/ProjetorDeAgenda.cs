using Chronos.Agenda.Application.Agendamentos.Contratos;
using Chronos.Agenda.Application.Agendamentos.Erros;
using Chronos.Agenda.Application.Disponibilidades.Contratos;
using Chronos.Agenda.Application.Organizacoes.Contratos;
using Chronos.Agenda.Application.Organizacoes.Erros;
using Chronos.Agenda.Application.Profissionais.Contratos;
using Chronos.Agenda.Application.Profissionais.Erros;
using Chronos.Agenda.Domain.Agendamentos.Entidades;
using Chronos.Agenda.Domain.Agendamentos.Enums;
using Chronos.Agenda.Domain.Agendamentos.ObjetosValor;
using Chronos.Agenda.Domain.Compartilhado.Resultados;
using Chronos.Agenda.Domain.Organizacoes.ObjetosValor;

namespace Chronos.Agenda.Application.Agendamentos;

/// <summary>
/// Resolve o fuso horário da organização corrente para um profissional e projeta a ocupação da sua agenda em um
/// dia local, cruzando disponibilidade semanal (RN07) e agendamentos ativos. Compartilhado entre as consultas
/// diária e semanal (UC07), que fazem a mesma resolução e a mesma projeção por dia.
/// </summary>
/// <example><code>
/// var projetor = new ProjetorDeAgenda(profissionalRepositorio, organizacaoRepositorio, disponibilidadeRepositorio, agendamentoRepositorio);
/// var fusoHorarioResultado = await projetor.ResolverFusoHorarioAsync(organizacaoId, profissionalId, cancellationToken);
/// var agendaDoDia = await projetor.ProjetarDiaAsync(organizacaoId, profissionalId, data, fusoHorarioResultado.Valor, cancellationToken);
/// </code></example>
internal sealed class ProjetorDeAgenda(
    IProfissionalRepositorio profissionalRepositorio,
    IOrganizacaoRepositorio organizacaoRepositorio,
    IDisponibilidadeSemanalRepositorio disponibilidadeRepositorio,
    IAgendamentoRepositorio agendamentoRepositorio)
{
    public async Task<Resultado<FusoHorario>> ResolverFusoHorarioAsync(Guid organizacaoId, Guid profissionalId, CancellationToken cancellationToken)
    {
        var profissional = await profissionalRepositorio.BuscarPorIdAsync(organizacaoId, profissionalId, cancellationToken);
        if (profissional is null)
        {
            return Resultado<FusoHorario>.Falha(ProfissionalErros.NaoEncontrado(organizacaoId, profissionalId));
        }

        var organizacao = await organizacaoRepositorio.BuscarPorIdAsync(organizacaoId, cancellationToken);
        if (organizacao is null)
        {
            return Resultado<FusoHorario>.Falha(OrganizacaoErros.NaoEncontrada(organizacaoId));
        }

        return organizacao.FusoHorario is null
            ? Resultado<FusoHorario>.Falha(AgendamentoErros.PerfilOperacionalNaoConfigurado(organizacaoId))
            : Resultado<FusoHorario>.Ok(organizacao.FusoHorario);
    }

    public async Task<AgendaDiariaResultado> ProjetarDiaAsync(
        Guid organizacaoId, Guid profissionalId, DateOnly data, FusoHorario fusoHorario, CancellationToken cancellationToken)
    {
        var diaDaSemana = data.DayOfWeek;
        var janelas = await disponibilidadeRepositorio.BuscarPorProfissionalEDiaAsync(organizacaoId, profissionalId, diaDaSemana, cancellationToken);
        var agendamentosProximos = await agendamentoRepositorio.BuscarPorProfissionalEPeriodoAsync(
            organizacaoId, profissionalId, IntervaloDeConsultaUtc(data), cancellationToken);

        var periodosOcupados = agendamentosProximos
            .Where(agendamento => agendamento.Status != StatusAgendamento.Cancelado && CaiNoDiaLocal(agendamento, data, fusoHorario))
            .Select(agendamento => ParaPeriodoOcupado(agendamento, fusoHorario))
            .OrderBy(periodo => periodo.Inicio)
            .ToList();

        var janelasOrdenadas = janelas.Select(disponibilidade => disponibilidade.Janela).OrderBy(janela => janela.Inicio).ToList();
        return new AgendaDiariaResultado(data, diaDaSemana, janelasOrdenadas, periodosOcupados);
    }

    /// <summary>Intervalo UTC usado apenas para reduzir a busca no repositório: alarga um dia para cada lado do
    /// dia local pedido, cobrindo qualquer offset de fuso horário possível. A data local exata de cada
    /// agendamento retornado é conferida depois, em <see cref="CaiNoDiaLocal"/>, sempre convertendo de UTC para
    /// local (nunca o inverso, que é ambíguo — ADR pendente #4).</summary>
    private static PeriodoAgendamento IntervaloDeConsultaUtc(DateOnly data)
    {
        var inicioUtc = DateTime.SpecifyKind(data.AddDays(-1).ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        var fimUtc = DateTime.SpecifyKind(data.AddDays(2).ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        return new PeriodoAgendamento(inicioUtc, fimUtc);
    }

    private static bool CaiNoDiaLocal(Agendamento agendamento, DateOnly data, FusoHorario fusoHorario)
    {
        var inicioLocal = fusoHorario.ConverterParaLocal(agendamento.Periodo.InicioUtc);
        return DateOnly.FromDateTime(inicioLocal.DateTime) == data;
    }

    private static PeriodoOcupado ParaPeriodoOcupado(Agendamento agendamento, FusoHorario fusoHorario)
    {
        var inicioLocal = fusoHorario.ConverterParaLocal(agendamento.Periodo.InicioUtc);
        var fimLocal = fusoHorario.ConverterParaLocal(agendamento.Periodo.FimUtc);
        return new PeriodoOcupado(
            agendamento.Id,
            agendamento.ServicoId,
            TimeOnly.FromDateTime(inicioLocal.DateTime),
            TimeOnly.FromDateTime(fimLocal.DateTime),
            agendamento.Status,
            agendamento.NomeServicoContratado,
            agendamento.PessoaAtendida.Nome.Valor,
            agendamento.PessoaAtendida.Tipo,
            agendamento.Local.Endereco?.Descricao);
    }
}
