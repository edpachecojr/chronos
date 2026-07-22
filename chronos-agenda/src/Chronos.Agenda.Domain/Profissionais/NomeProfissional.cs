using Chronos.Agenda.Domain.Compartilhado;

namespace Chronos.Agenda.Domain.Profissionais;

/// <summary>Representa o nome de exibição de um profissional.</summary>
public sealed record NomeProfissional
{
    public NomeProfissional(string valor)
    {
        var normalizado = valor?.Trim();
        if (string.IsNullOrWhiteSpace(normalizado) || normalizado.Length > 120)
        {
            throw new DomainException("O nome do profissional deve ter entre 1 e 120 caracteres.");
        }

        Valor = normalizado;
    }

    public string Valor { get; }
}
