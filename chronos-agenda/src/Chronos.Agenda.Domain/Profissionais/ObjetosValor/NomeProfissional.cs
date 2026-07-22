using Chronos.Agenda.Domain.Profissionais.Excecoes;

namespace Chronos.Agenda.Domain.Profissionais.ObjetosValor;

/// <summary>Representa o nome de exibição de um profissional.</summary>
public sealed record NomeProfissional
{
    public NomeProfissional(string valor)
    {
        var normalizado = valor?.Trim();
        if (string.IsNullOrWhiteSpace(normalizado) || normalizado.Length > 120)
        {
            throw new NomeProfissionalInvalidoException(normalizado?.Length ?? 0);
        }

        Valor = normalizado;
    }

    public string Valor { get; }
}
