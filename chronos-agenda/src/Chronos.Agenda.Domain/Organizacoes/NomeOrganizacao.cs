using Chronos.Agenda.Domain.Compartilhado;

namespace Chronos.Agenda.Domain.Organizacoes;

/// <summary>Representa o nome pelo qual um negócio é identificado.</summary>
public sealed record NomeOrganizacao
{
    public NomeOrganizacao(string valor)
    {
        var normalizado = valor?.Trim();
        if (string.IsNullOrWhiteSpace(normalizado) || normalizado.Length > 120)
        {
            throw new DomainException("O nome da organização deve ter entre 1 e 120 caracteres.");
        }

        Valor = normalizado;
    }

    public string Valor { get; }
}
