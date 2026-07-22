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
