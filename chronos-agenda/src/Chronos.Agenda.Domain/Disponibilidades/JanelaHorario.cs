using Chronos.Agenda.Domain.Compartilhado;

namespace Chronos.Agenda.Domain.Disponibilidades;

/// <summary>Representa uma janela contínua de atendimento dentro de um dia.</summary>
public sealed record JanelaHorario
{
    public JanelaHorario(TimeOnly inicio, TimeOnly fim)
    {
        if (fim <= inicio)
        {
            throw new DomainException("O fim da janela de atendimento deve ser posterior ao início.");
        }

        Inicio = inicio;
        Fim = fim;
    }

    public TimeOnly Inicio { get; }
    public TimeOnly Fim { get; }
}
