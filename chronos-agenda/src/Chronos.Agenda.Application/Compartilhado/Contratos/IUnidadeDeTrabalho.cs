namespace Chronos.Agenda.Application.Compartilhado.Contratos;

/// <summary>Garante que as operações de um caso de uso sejam persistidas atomicamente.</summary>
public interface IUnidadeDeTrabalho
{
    /// <summary>Confirma, numa única transação, as alterações pendentes dos repositórios usados no caso de uso.</summary>
    /// <example><code>await unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);</code></example>
    Task SalvarAlteracoesAsync(CancellationToken cancellationToken);
}
