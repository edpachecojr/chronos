using Chronos.Agenda.Domain.Disponibilidades.Excecoes;

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
}
