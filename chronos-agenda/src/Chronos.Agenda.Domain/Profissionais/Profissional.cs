using Chronos.Agenda.Domain.Compartilhado;

namespace Chronos.Agenda.Domain.Profissionais;

/// <summary>Representa quem presta serviços dentro de uma organização.</summary>
public sealed class Profissional : Entidade, IPertenceOrganizacao
{
    public Profissional(Guid id, Guid organizacaoId, NomeProfissional nome, DateTime criadoEmUtc)
        : base(id, criadoEmUtc)
    {
        if (organizacaoId == Guid.Empty)
        {
            throw new DomainException("O profissional deve pertencer a uma organização válida.");
        }

        OrganizacaoId = organizacaoId;
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
    }

    public Guid OrganizacaoId { get; }
    public NomeProfissional Nome { get; private set; }

    public void Renomear(NomeProfissional nome, DateTime atualizadoEmUtc)
    {
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        RegistrarAtualizacao(atualizadoEmUtc);
    }
}
