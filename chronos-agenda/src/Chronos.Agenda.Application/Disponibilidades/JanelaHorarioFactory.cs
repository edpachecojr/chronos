using Chronos.Agenda.Application.Disponibilidades.Erros;
using Chronos.Agenda.Domain.Compartilhado.Resultados;
using Chronos.Agenda.Domain.Disponibilidades.Exceptions;
using Chronos.Agenda.Domain.Disponibilidades.ObjetosValor;

namespace Chronos.Agenda.Application.Disponibilidades;

/// <summary>Constrói uma <see cref="JanelaHorario"/> convertendo a exceção de domínio em erro esperado da
/// aplicação, reaproveitado pelos casos de uso de criar e alterar disponibilidade.</summary>
internal static class JanelaHorarioFactory
{
    /// <example><code>var resultado = JanelaHorarioFactory.Criar(comando.Inicio, comando.Fim);</code></example>
    public static Resultado<JanelaHorario> Criar(TimeOnly inicio, TimeOnly fim)
    {
        try
        {
            return Resultado<JanelaHorario>.Ok(new JanelaHorario(inicio, fim));
        }
        catch (JanelaHorarioInvalidaException excecao)
        {
            return Resultado<JanelaHorario>.Falha(DisponibilidadeErros.JanelaInvalida(excecao.Message));
        }
    }
}
