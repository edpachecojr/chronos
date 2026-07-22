using Chronos.Agenda.Domain.Compartilhado.Resultados;
using Chronos.Agenda.Domain.Disponibilidades.ObjetosValor;

namespace Chronos.Agenda.Domain.Disponibilidades.Erros;

/// <summary>Catálogo de erros esperados nas operações de disponibilidade.</summary>
public static class DisponibilidadeErros
{
    /// <summary>O período ocupado não está contido em nenhuma disponibilidade do profissional no dia informado (RN07).</summary>
    public static Erro ForaDaJanela(DayOfWeek diaDaSemana, JanelaHorario janelaOcupada) => new(
        "Disponibilidade.ForaDaJanela",
        $"O período ({janelaOcupada.Inicio}-{janelaOcupada.Fim}) em {diaDaSemana} não está dentro de nenhuma disponibilidade do profissional.");
}
