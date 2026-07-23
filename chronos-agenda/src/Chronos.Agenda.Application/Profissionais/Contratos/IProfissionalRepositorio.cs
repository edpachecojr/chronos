using Chronos.Agenda.Domain.Profissionais.Entidades;

namespace Chronos.Agenda.Application.Profissionais.Contratos;

/// <summary>Acesso a profissionais, restrito aos métodos que os casos de uso do MVP utilizam (sem CRUD genérico).</summary>
public interface IProfissionalRepositorio
{
    /// <summary>Persiste um novo profissional.</summary>
    Task AdicionarAsync(Profissional profissional, CancellationToken cancellationToken);

    /// <summary>Busca um profissional dentro do limite da organização informada (RN01); retorna nulo se não existir
    /// ou pertencer a outra organização.</summary>
    Task<Profissional?> BuscarPorIdAsync(Guid organizacaoId, Guid profissionalId, CancellationToken cancellationToken);

    /// <summary>Busca todos os profissionais vinculados à organização informada, usados para listar o time da
    /// organização corrente.</summary>
    Task<IReadOnlyCollection<Profissional>> BuscarPorOrganizacaoAsync(Guid organizacaoId, CancellationToken cancellationToken);
}
