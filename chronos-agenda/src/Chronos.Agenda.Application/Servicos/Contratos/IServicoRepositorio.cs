using Chronos.Agenda.Domain.Servicos.Entidades;

namespace Chronos.Agenda.Application.Servicos.Contratos;

/// <summary>Acesso a serviços, restrito aos métodos que os casos de uso do MVP utilizam (sem CRUD genérico).</summary>
public interface IServicoRepositorio
{
    /// <summary>Persiste um novo serviço.</summary>
    Task AdicionarAsync(Servico servico, CancellationToken cancellationToken);

    /// <summary>Persiste a configuração comercial atualizada de um serviço.</summary>
    Task AtualizarAsync(Servico servico, CancellationToken cancellationToken);

    /// <summary>Busca um serviço dentro do limite da organização informada (RN01); retorna nulo se não existir ou
    /// pertencer a outra organização.</summary>
    Task<Servico?> BuscarPorIdAsync(Guid organizacaoId, Guid servicoId, CancellationToken cancellationToken);

    /// <summary>Busca os serviços do catálogo de um profissional, dentro do limite da organização informada
    /// (RN01), usados para listar o catálogo (UC03).</summary>
    Task<IReadOnlyCollection<Servico>> BuscarPorProfissionalAsync(Guid organizacaoId, Guid profissionalId, CancellationToken cancellationToken);
}
