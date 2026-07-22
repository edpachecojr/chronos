using Chronos.Agenda.Domain.Agendamentos.ObjetosValor;
using Chronos.Agenda.Domain.Disponibilidades.Entidades;
using Chronos.Agenda.Domain.Disponibilidades.Exceptions;
using Chronos.Agenda.Domain.Organizacoes.ObjetosValor;

namespace Chronos.Agenda.Domain.Disponibilidades.Servicos;

/// <summary>Verifica se um período de agendamento cabe na disponibilidade semanal do profissional (RN07).</summary>
public static class VerificadorDisponibilidade
{
    /// <example><code>VerificadorDisponibilidade.Verificar(periodo, organizacao.FusoHorario, disponibilidadesDoProfissional);</code></example>
    public static void Verificar(PeriodoAgendamento periodo, FusoHorario fusoHorario, IReadOnlyCollection<DisponibilidadeSemanal> disponibilidades)
    {
        ArgumentNullException.ThrowIfNull(periodo);
        ArgumentNullException.ThrowIfNull(fusoHorario);
        ArgumentNullException.ThrowIfNull(disponibilidades);

        // Conversão UTC -> local nunca é ambígua ou inexistente (isso só ocorre no sentido
        // local -> UTC, tratado na aplicação ao interpretar o horário informado pelo usuário).
        var inicioLocal = fusoHorario.ConverterParaLocal(periodo.InicioUtc);
        var fimLocal = fusoHorario.ConverterParaLocal(periodo.FimUtc);

        if (!CabeEmAlgumaJanela(inicioLocal, fimLocal, disponibilidades))
        {
            throw new PeriodoForaDaDisponibilidadeException(periodo.InicioUtc, periodo.FimUtc);
        }
    }

    private static bool CabeEmAlgumaJanela(DateTime inicioLocal, DateTime fimLocal, IReadOnlyCollection<DisponibilidadeSemanal> disponibilidades)
    {
        if (inicioLocal.Date != fimLocal.Date)
        {
            return false;
        }

        var inicioHorario = TimeOnly.FromDateTime(inicioLocal);
        var fimHorario = TimeOnly.FromDateTime(fimLocal);
        return disponibilidades
            .Where(disponibilidade => disponibilidade.DiaDaSemana == inicioLocal.DayOfWeek)
            .Any(disponibilidade => disponibilidade.Janela.Inicio <= inicioHorario && fimHorario <= disponibilidade.Janela.Fim);
    }
}
