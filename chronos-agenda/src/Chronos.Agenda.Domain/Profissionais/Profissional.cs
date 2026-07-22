using Chronos.Agenda.Domain.Compartilhado;

namespace Chronos.Agenda.Domain.Profissionais;

/// <summary>Representa quem presta serviços dentro de uma organização.</summary>
public sealed class Profissional : Entidade, IPertenceOrganizacao
{
    private Profissional(
        Guid id,
        Guid organizacaoId,
        NomeProfissional nome,
        DateTime criadoEmUtc,
        DateTime atualizadoEmUtc)
        : base(id, criadoEmUtc, atualizadoEmUtc)
    {
        OrganizacaoId = organizacaoId;
        Nome = nome;
    }

    public Guid OrganizacaoId { get; }
    public NomeProfissional Nome { get; private set; }

    /// <summary>Cria um novo profissional vinculado a uma organização.</summary>
    /// <example><code>var profissional = Profissional.Criar(organizacaoId, nome, agoraUtc);</code></example>
    public static Profissional Criar(Guid organizacaoId, NomeProfissional nome, DateTime criadoEmUtc)
    {
        ValidarCriacao(criadoEmUtc);
        ValidarOrganizacao(organizacaoId);
        var profissional = new Profissional(Guid.NewGuid(), organizacaoId, nome, criadoEmUtc, criadoEmUtc);
        profissional.LancarEventoDominio(new ProfissionalCriadoEventoDominio(profissional.Id, organizacaoId, criadoEmUtc));
        return profissional;
    }

    /// <summary>Reconstitui um profissional previamente persistido, sem executar regras de criação.</summary>
    /// <example><code>var profissional = Profissional.Reidratar(id, organizacaoId, nome, criadoEmUtc, atualizadoEmUtc);</code></example>
    public static Profissional Reidratar(Guid id, Guid organizacaoId, NomeProfissional nome, DateTime criadoEmUtc, DateTime atualizadoEmUtc)
    {
        return new Profissional(id, organizacaoId, nome, criadoEmUtc, atualizadoEmUtc);
    }

    public void Renomear(NomeProfissional nome, DateTime atualizadoEmUtc)
    {
        Nome = nome;
        RegistrarAtualizacao(atualizadoEmUtc);
    }

    private static void ValidarOrganizacao(Guid organizacaoId)
    {
        if (organizacaoId == Guid.Empty)
        {
            throw new DomainException("O profissional deve pertencer a uma organização válida.");
        }
    }
}
