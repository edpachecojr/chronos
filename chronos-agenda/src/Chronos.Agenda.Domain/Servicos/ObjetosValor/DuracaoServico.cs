using Chronos.Agenda.Domain.Servicos.Excecoes;

namespace Chronos.Agenda.Domain.Servicos.ObjetosValor;

/// <summary>Representa o tempo reservado na agenda para a execução de um serviço.</summary>
public sealed record DuracaoServico
{
    public DuracaoServico(TimeSpan valor)
    {
        if (valor <= TimeSpan.Zero || valor > TimeSpan.FromHours(12))
        {
            throw new DuracaoServicoInvalidaException(valor);
        }

        Valor = valor;
    }

    public TimeSpan Valor { get; }
}
