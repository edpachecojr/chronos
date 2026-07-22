using Chronos.Agenda.Domain.Compartilhado.Exceptions;

namespace Chronos.Agenda.Domain.Compartilhado.ObjetosValor;

/// <summary>Representa o nome próprio de uma pessoa, compartilhado entre profissional, pessoa atendida e demais papéis que exigirem identificação nominal.</summary>
public sealed record Nome
{
    public Nome(string valor)
    {
        var normalizado = valor?.Trim();
        if (string.IsNullOrWhiteSpace(normalizado) || normalizado.Length > 120)
        {
            throw new NomeInvalidoException(normalizado?.Length ?? 0);
        }

        Valor = normalizado;
    }

    public string Valor { get; }
}
