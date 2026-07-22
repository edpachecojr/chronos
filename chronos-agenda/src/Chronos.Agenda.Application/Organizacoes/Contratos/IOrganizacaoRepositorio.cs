using Chronos.Agenda.Domain.Organizacoes.Entidades;

namespace Chronos.Agenda.Application.Organizacoes.Contratos;

/// <summary>Acesso a organizações, restrito aos métodos que os casos de uso do MVP utilizam (sem CRUD genérico).</summary>
public interface IOrganizacaoRepositorio
{
    /// <summary>Persiste uma nova organização.</summary>
    Task AdicionarAsync(Organizacao organizacao, CancellationToken cancellationToken);

    /// <summary>Busca uma organização pelo identificador, que é o próprio limite de tenant (RN01).</summary>
    Task<Organizacao?> BuscarPorIdAsync(Guid organizacaoId, CancellationToken cancellationToken);
}
