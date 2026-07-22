using Chronos.Agenda.Domain.Servicos.Excecoes;

namespace Chronos.Agenda.Domain.Servicos.ObjetosValor;

/// <summary>Representa o preço cobrado por um serviço, em reais.</summary>
public sealed record PrecoServico
{
    public PrecoServico(decimal valor)
    {
        if (valor < 0 || decimal.Round(valor, 2) != valor)
        {
            throw new PrecoServicoInvalidoException(valor);
        }

        Valor = valor;
    }

    public decimal Valor { get; }
}
