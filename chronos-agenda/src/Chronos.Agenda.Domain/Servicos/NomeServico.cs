using Chronos.Agenda.Domain.Compartilhado;

namespace Chronos.Agenda.Domain.Servicos;

/// <summary>Representa o nome comercial de um serviço oferecido.</summary>
public sealed record NomeServico
{
    public NomeServico(string valor)
    {
        var normalizado = valor?.Trim();
        if (string.IsNullOrWhiteSpace(normalizado) || normalizado.Length > 120)
        {
            throw new DomainException("O nome do serviço deve ter entre 1 e 120 caracteres.");
        }

        Valor = normalizado;
    }

    public string Valor { get; }
}
