using Chronos.Agenda.Application.Compartilhado.Contratos;
using Chronos.Agenda.Infrastructure.Data;

namespace Chronos.Agenda.Infrastructure.Compartilhado;

/// <summary>Confirma, numa única transação do EF Core, as alterações pendentes no <see cref="ChronosAgendaDbContext"/>
/// compartilhado pelos repositórios da requisição corrente.</summary>
public sealed class UnidadeDeTrabalho(ChronosAgendaDbContext dbContext) : IUnidadeDeTrabalho
{
    public async Task SalvarAlteracoesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
