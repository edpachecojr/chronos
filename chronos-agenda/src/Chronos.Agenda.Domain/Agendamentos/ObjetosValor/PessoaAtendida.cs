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

    /// <summary>Construtor sem parâmetros usado apenas pelo EF Core para materialização: <see cref="Nome"/> é, em
    /// si, outro objeto de valor mapeado, e não pode ser vinculado a um parâmetro de construtor pelo EF Core.</summary>
    private PessoaAtendida()
    {
        Nome = null!;
    }

    public Nome Nome { get; }
    public TipoPessoaAtendida Tipo { get; }
}
