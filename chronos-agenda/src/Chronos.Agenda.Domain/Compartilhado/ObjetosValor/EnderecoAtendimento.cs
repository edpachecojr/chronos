using Chronos.Agenda.Domain.Compartilhado.Exceptions;

namespace Chronos.Agenda.Domain.Compartilhado.ObjetosValor;

/// <summary>Representa o endereço físico informado para a prestação do serviço.</summary>
public sealed record EnderecoAtendimento
{
    public EnderecoAtendimento(string descricao)
    {
        var descricaoNormalizada = descricao?.Trim();
        if (string.IsNullOrWhiteSpace(descricaoNormalizada) || descricaoNormalizada.Length > 300)
        {
            throw new EnderecoAtendimentoInvalidoException(descricaoNormalizada?.Length ?? 0);
        }

        Descricao = descricaoNormalizada;
    }

    public string Descricao { get; }
}
