using Chronos.Agenda.Domain.Agendamentos.Entidades;
using Chronos.Agenda.Domain.Agendamentos.ObjetosValor;

namespace Chronos.Agenda.Application.Agendamentos.Contratos;

/// <summary>Acesso a agendamentos, restrito aos métodos que os casos de uso do MVP utilizam (sem CRUD genérico).</summary>
public interface IAgendamentoRepositorio
{
    /// <summary>Persiste um novo agendamento.</summary>
    Task AdicionarAsync(Agendamento agendamento, CancellationToken cancellationToken);

    /// <summary>Persiste os dados atualizados de um agendamento (reagendamento, confirmação ou cancelamento).</summary>
    Task AtualizarAsync(Agendamento agendamento, CancellationToken cancellationToken);

    /// <summary>Busca um agendamento dentro do limite da organização informada (RN01); retorna nulo se não existir
    /// ou pertencer a outra organização.</summary>
    Task<Agendamento?> BuscarPorIdAsync(Guid organizacaoId, Guid agendamentoId, CancellationToken cancellationToken);

    /// <summary>Busca agendamentos ativos do profissional, dentro do limite da organização informada (RN01), que se
    /// sobrepõem ao período informado, para impedir conflito de agenda (RN02).
    /// <paramref name="excluirAgendamentoId"/> remove o próprio agendamento da busca ao reagendar (UC05); passe
    /// <c>null</c> ao criar um agendamento novo (UC04).</summary>
    Task<IReadOnlyCollection<Agendamento>> BuscarAtivosSobrepostosAsync(
        Guid organizacaoId, Guid profissionalId, PeriodoAgendamento periodo, Guid? excluirAgendamentoId, CancellationToken cancellationToken);

    /// <summary>Busca agendamentos do profissional, dentro do limite da organização informada (RN01), num período,
    /// para projeção de agenda (UC07).</summary>
    Task<IReadOnlyCollection<Agendamento>> BuscarPorProfissionalEPeriodoAsync(
        Guid organizacaoId, Guid profissionalId, PeriodoAgendamento periodo, CancellationToken cancellationToken);
}
