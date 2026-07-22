using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Servicos.Enums;

namespace Chronos.Agenda.Domain.Agendamentos.ObjetosValor;

/// <summary>Define onde um serviço será prestado, conforme a sua modalidade.</summary>
public sealed record LocalAtendimento
{
    private LocalAtendimento(TipoAtendimento tipo, EnderecoAtendimento? endereco)
    {
        Tipo = tipo;
        Endereco = endereco;
    }

    /// <summary>Construtor sem parâmetros usado apenas pelo EF Core para materialização: <see cref="Endereco"/> é,
    /// em si, outro objeto de valor mapeado, e não pode ser vinculado a um parâmetro de construtor pelo EF Core.</summary>
    private LocalAtendimento()
    {
    }

    public TipoAtendimento Tipo { get; }
    public EnderecoAtendimento? Endereco { get; }

    /// <summary>Cria o local de um atendimento online, sem endereço físico.</summary>
    /// <example><code>var local = LocalAtendimento.Online();</code></example>
    public static LocalAtendimento Online() => new(TipoAtendimento.Online, null);

    /// <summary>Cria o local de um atendimento no endereço da pessoa atendida.</summary>
    /// <example><code>var local = LocalAtendimento.Domiciliar(new EnderecoAtendimento("Rua Exemplo, 10"));</code></example>
    public static LocalAtendimento Domiciliar(EnderecoAtendimento endereco) => new(TipoAtendimento.Domiciliar, endereco ?? throw new ArgumentNullException(nameof(endereco)));

    /// <summary>Cria o local de um atendimento no endereço do prestador.</summary>
    /// <example><code>var local = LocalAtendimento.NoEnderecoDoPrestador(new EnderecoAtendimento("Av. Central, 20"));</code></example>
    public static LocalAtendimento NoEnderecoDoPrestador(EnderecoAtendimento endereco) => new(TipoAtendimento.NoEnderecoDoPrestador, endereco ?? throw new ArgumentNullException(nameof(endereco)));
}
