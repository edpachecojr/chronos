using Chronos.Agenda.Domain.Agendamentos.Excecoes;

namespace Chronos.Agenda.Domain.Agendamentos.ObjetosValor;

/// <summary>Representa o nome informado para identificar o cliente em um agendamento.</summary>
public sealed record NomeCliente
{
    public NomeCliente(string valor)
    {
        var normalizado = valor?.Trim();
        if (string.IsNullOrWhiteSpace(normalizado) || normalizado.Length > 120)
        {
            throw new NomeClienteInvalidoException(normalizado?.Length ?? 0);
        }

        Valor = normalizado;
    }

    public string Valor { get; }
}
