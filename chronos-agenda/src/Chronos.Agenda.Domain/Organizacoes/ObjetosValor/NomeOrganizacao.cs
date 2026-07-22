using Chronos.Agenda.Domain.Organizacoes.Exceptions;

namespace Chronos.Agenda.Domain.Organizacoes.ObjetosValor;

/// <summary>Representa o nome pelo qual um negócio é identificado.</summary>
public sealed record NomeOrganizacao
{
    public NomeOrganizacao(string valor)
    {
        var normalizado = valor?.Trim();
        if (string.IsNullOrWhiteSpace(normalizado) || normalizado.Length > 120)
        {
            throw new NomeOrganizacaoInvalidoException(normalizado?.Length ?? 0);
        }

        Valor = normalizado;
    }

    public string Valor { get; }
}
