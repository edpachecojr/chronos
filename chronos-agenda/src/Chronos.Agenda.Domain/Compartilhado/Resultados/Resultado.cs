namespace Chronos.Agenda.Domain.Compartilhado.Resultados;

/// <summary>Representa o resultado de uma operação de domínio cuja falha é um cenário esperado do fluxo de negócio.</summary>
public sealed class Resultado
{
    private Resultado(bool sucesso, Erro? erro)
    {
        Sucesso = sucesso;
        Erro = erro;
    }

    public bool Sucesso { get; }
    public bool Falhou => !Sucesso;
    public Erro? Erro { get; }

    /// <summary>Cria um resultado bem-sucedido.</summary>
    /// <example><code>var resultado = Resultado.Ok();</code></example>
    public static Resultado Ok() => new(true, null);

    /// <summary>Cria um resultado de falha com o erro correspondente.</summary>
    /// <example><code>var resultado = Resultado.Falha(AgendamentoErros.JaCancelado);</code></example>
    public static Resultado Falha(Erro erro) => new(false, erro);
}

/// <summary>Representa o resultado de uma operação que, quando bem-sucedida, produz um valor. Reaproveita o mesmo
/// catálogo de <see cref="Erro"/> do Result Pattern não genérico, sem duplicar o conceito.</summary>
public sealed class Resultado<T>
{
    private readonly T? _valor;

    private Resultado(bool sucesso, T? valor, Erro? erro)
    {
        Sucesso = sucesso;
        _valor = valor;
        Erro = erro;
    }

    public bool Sucesso { get; }
    public bool Falhou => !Sucesso;
    public Erro? Erro { get; }

    /// <summary>Valor produzido pela operação bem-sucedida. Lançar quando <see cref="Falhou"/> indica uso incorreto
    /// da API pelo chamador, não um erro de domínio.</summary>
    public T Valor => Sucesso
        ? _valor!
        : throw new InvalidOperationException("Não é possível obter o valor de um resultado com falha.");

    /// <summary>Cria um resultado bem-sucedido com o valor produzido.</summary>
    /// <example><code>var resultado = Resultado&lt;Guid&gt;.Ok(organizacaoId);</code></example>
    public static Resultado<T> Ok(T valor) => new(true, valor, null);

    /// <summary>Cria um resultado de falha com o erro correspondente.</summary>
    /// <example><code>var resultado = Resultado&lt;Guid&gt;.Falha(OrganizacaoErros.NaoEncontrada);</code></example>
    public static Resultado<T> Falha(Erro erro) => new(false, default, erro);
}
