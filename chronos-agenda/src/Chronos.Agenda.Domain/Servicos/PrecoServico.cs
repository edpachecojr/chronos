using Chronos.Agenda.Domain.Compartilhado;

namespace Chronos.Agenda.Domain.Servicos;

/// <summary>Representa o preço cobrado por um serviço, em reais.</summary>
public sealed record PrecoServico
{
    public PrecoServico(decimal valor)
    {
        if (valor < 0 || decimal.Round(valor, 2) != valor)
        {
            throw new DomainException("O preço do serviço deve ser não negativo e ter no máximo duas casas decimais.");
        }

        Valor = valor;
    }

    public decimal Valor { get; }
}
