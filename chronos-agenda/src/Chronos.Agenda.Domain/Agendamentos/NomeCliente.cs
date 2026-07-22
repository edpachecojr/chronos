using Chronos.Agenda.Domain.Compartilhado;

namespace Chronos.Agenda.Domain.Agendamentos;

/// <summary>Representa o nome informado para identificar o cliente em um agendamento.</summary>
public sealed record NomeCliente
{
    public NomeCliente(string valor)
    {
        var normalizado = valor?.Trim();
        if (string.IsNullOrWhiteSpace(normalizado) || normalizado.Length > 120)
        {
            throw new DomainException("O nome do cliente deve ter entre 1 e 120 caracteres.");
        }

        Valor = normalizado;
    }

    public string Valor { get; }
}
