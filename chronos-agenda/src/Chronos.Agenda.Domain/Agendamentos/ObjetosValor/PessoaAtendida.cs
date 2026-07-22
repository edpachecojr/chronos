using Chronos.Agenda.Domain.Agendamentos.Enums;
using Chronos.Agenda.Domain.Agendamentos.Exceptions;

namespace Chronos.Agenda.Domain.Agendamentos.ObjetosValor;

/// <summary>Identifica a pessoa que receberá o serviço em um agendamento.</summary>
public sealed record PessoaAtendida
{
    public PessoaAtendida(string nome, TipoPessoaAtendida tipo)
    {
        var nomeNormalizado = nome?.Trim();
        if (string.IsNullOrWhiteSpace(nomeNormalizado) || nomeNormalizado.Length > 120)
        {
            throw new NomePessoaAtendidaInvalidoException(nomeNormalizado?.Length ?? 0);
        }

        Nome = nomeNormalizado;
        Tipo = tipo;
    }

    public string Nome { get; }
    public TipoPessoaAtendida Tipo { get; }
}
