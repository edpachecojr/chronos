using Chronos.Agenda.Domain.Compartilhado.Resultados;
using Chronos.Agenda.Domain.Disponibilidades.Entidades;
using Chronos.Agenda.Domain.Disponibilidades.Erros;
using Chronos.Agenda.Domain.Disponibilidades.ObjetosValor;

namespace Chronos.Agenda.Domain.Disponibilidades.Servicos;

/// <summary>Verifica se um período ocupado cabe na disponibilidade semanal do profissional (RN07). Recebe o dia da
/// semana e a janela já convertidos para o horário local da organização; a conversão UTC-local é responsabilidade
/// da camada de aplicação.</summary>
public static class VerificadorDisponibilidade
{
    /// <example><code>var resultado = VerificadorDisponibilidade.Verificar(diaDaSemanaLocal, janelaOcupadaLocal, disponibilidadesDoProfissional);</code></example>
    public static Resultado Verificar(DayOfWeek diaDaSemana, JanelaHorario janelaOcupada, IReadOnlyCollection<DisponibilidadeSemanal> disponibilidades)
    {
        ArgumentNullException.ThrowIfNull(janelaOcupada);
        ArgumentNullException.ThrowIfNull(disponibilidades);

        return CabeEmAlgumaJanela(diaDaSemana, janelaOcupada, disponibilidades)
            ? Resultado.Ok()
            : Resultado.Falha(DisponibilidadeErros.ForaDaJanela(diaDaSemana, janelaOcupada));
    }

    private static bool CabeEmAlgumaJanela(DayOfWeek diaDaSemana, JanelaHorario janelaOcupada, IReadOnlyCollection<DisponibilidadeSemanal> disponibilidades)
    {
        return disponibilidades
            .Where(disponibilidade => disponibilidade.DiaDaSemana == diaDaSemana)
            .Any(disponibilidade => disponibilidade.Janela.Inicio <= janelaOcupada.Inicio && janelaOcupada.Fim <= disponibilidade.Janela.Fim);
    }
}
