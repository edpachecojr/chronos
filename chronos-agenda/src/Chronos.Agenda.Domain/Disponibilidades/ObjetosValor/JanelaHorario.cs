using Chronos.Agenda.Domain.Disponibilidades.Exceptions;

namespace Chronos.Agenda.Domain.Disponibilidades.ObjetosValor;

/// <summary>Representa uma janela contínua de atendimento dentro de um dia.</summary>
public sealed record JanelaHorario
{
    public JanelaHorario(TimeOnly inicio, TimeOnly fim)
    {
        if (fim <= inicio)
        {
            throw new JanelaHorarioInvalidaException(inicio, fim);
        }

        Inicio = inicio;
        Fim = fim;
    }

    public TimeOnly Inicio { get; }
    public TimeOnly Fim { get; }

    /// <summary>Informa se esta janela tem algum instante em comum com outra.</summary>
    /// <example><code>var sobrepoe = janela.Sobrepoe(outraJanela);</code></example>
    public bool Sobrepoe(JanelaHorario outra)
    {
        ArgumentNullException.ThrowIfNull(outra);
        return Inicio < outra.Fim && outra.Inicio < Fim;
    }
}
