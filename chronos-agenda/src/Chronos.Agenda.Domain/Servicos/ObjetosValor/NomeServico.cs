using Chronos.Agenda.Domain.Servicos.Excecoes;

namespace Chronos.Agenda.Domain.Servicos.ObjetosValor;

/// <summary>Representa o nome comercial de um serviço oferecido.</summary>
public sealed record NomeServico
{
    public NomeServico(string valor)
    {
        var normalizado = valor?.Trim();
        if (string.IsNullOrWhiteSpace(normalizado) || normalizado.Length > 120)
        {
            throw new NomeServicoInvalidoException(normalizado?.Length ?? 0);
        }

        Valor = normalizado;
    }

    public string Valor { get; }
}
