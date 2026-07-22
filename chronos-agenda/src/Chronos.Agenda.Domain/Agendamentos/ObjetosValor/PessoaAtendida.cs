using Chronos.Agenda.Domain.Agendamentos.Enums;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;

namespace Chronos.Agenda.Domain.Agendamentos.ObjetosValor;

/// <summary>Identifica a pessoa que receberá o serviço em um agendamento.</summary>
public sealed record PessoaAtendida
{
    public PessoaAtendida(Nome nome, TipoPessoaAtendida tipo)
    {
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        Tipo = tipo;
    }

    public Nome Nome { get; }
    public TipoPessoaAtendida Tipo { get; }
}
