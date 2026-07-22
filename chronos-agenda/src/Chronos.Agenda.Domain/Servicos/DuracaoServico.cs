using Chronos.Agenda.Domain.Compartilhado;

namespace Chronos.Agenda.Domain.Servicos;

/// <summary>Representa o tempo reservado na agenda para a execução de um serviço.</summary>
public sealed record DuracaoServico
{
    public DuracaoServico(TimeSpan valor)
    {
        if (valor <= TimeSpan.Zero || valor > TimeSpan.FromHours(12))
        {
            throw new DomainException("A duração do serviço deve ser maior que zero e de no máximo 12 horas.");
        }

        Valor = valor;
    }

    public TimeSpan Valor { get; }
}
