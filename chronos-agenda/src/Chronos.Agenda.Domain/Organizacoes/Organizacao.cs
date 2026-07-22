using Chronos.Agenda.Domain.Compartilhado;

namespace Chronos.Agenda.Domain.Organizacoes;

/// <summary>Representa o limite de propriedade e acesso do negócio no Chronos.</summary>
public sealed class Organizacao : Entidade
{
    private Organizacao(Guid id, NomeOrganizacao nome, DateTime criadoEmUtc, DateTime atualizadoEmUtc)
        : base(id, criadoEmUtc, atualizadoEmUtc)
    {
        Nome = nome;
    }

    public NomeOrganizacao Nome { get; private set; }

    /// <summary>Cria uma nova organização.</summary>
    /// <example><code>var organizacao = Organizacao.Criar(nome, agoraUtc);</code></example>
    public static Organizacao Criar(NomeOrganizacao nome, DateTime criadoEmUtc)
    {
        ValidarCriacao(criadoEmUtc);
        return new Organizacao(Guid.NewGuid(), nome, criadoEmUtc, criadoEmUtc);
    }

    /// <summary>Reconstitui uma organização previamente persistida, sem executar regras de criação.</summary>
    /// <example><code>var organizacao = Organizacao.Reidratar(id, nome, criadoEmUtc, atualizadoEmUtc);</code></example>
    public static Organizacao Reidratar(Guid id, NomeOrganizacao nome, DateTime criadoEmUtc, DateTime atualizadoEmUtc)
    {
        return new Organizacao(id, nome, criadoEmUtc, atualizadoEmUtc);
    }

    public void Renomear(NomeOrganizacao nome, DateTime atualizadoEmUtc)
    {
        Nome = nome;
        RegistrarAtualizacao(atualizadoEmUtc);
    }
}
