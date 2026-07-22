namespace Chronos.Agenda.Domain.Compartilhado;

/// <summary>Representa um objeto do domínio com identidade e auditoria comuns.</summary>
public abstract class Entidade
{
    protected Entidade(Guid id, DateTime criadoEmUtc)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("A identidade da entidade não pode ser vazia.");
        }

        Id = id;
        CriadoEmUtc = ExigirUtc(criadoEmUtc, nameof(criadoEmUtc));
        AtualizadoEmUtc = CriadoEmUtc;
    }

    public Guid Id { get; }
    public DateTime CriadoEmUtc { get; }
    public DateTime AtualizadoEmUtc { get; private set; }

    protected void RegistrarAtualizacao(DateTime atualizadoEmUtc)
    {
        var instante = ExigirUtc(atualizadoEmUtc, nameof(atualizadoEmUtc));
        if (instante < CriadoEmUtc)
        {
            throw new DomainException("A atualização não pode ser anterior à criação da entidade.");
        }

        AtualizadoEmUtc = instante;
    }

    private static DateTime ExigirUtc(DateTime instante, string nomeParametro)
    {
        if (instante.Kind != DateTimeKind.Utc)
        {
            throw new DomainException($"{nomeParametro} deve estar em UTC.");
        }

        return instante;
    }
}
