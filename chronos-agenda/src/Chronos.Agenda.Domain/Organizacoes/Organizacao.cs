using Chronos.Agenda.Domain.Compartilhado;

namespace Chronos.Agenda.Domain.Organizacoes;

/// <summary>Representa o limite de propriedade e acesso do negócio no Chronos.</summary>
public sealed class Organizacao : Entidade
{
    public Organizacao(Guid id, NomeOrganizacao nome, DateTime criadoEmUtc)
        : base(id, criadoEmUtc)
    {
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
    }

    public NomeOrganizacao Nome { get; private set; }

    public void Renomear(NomeOrganizacao nome, DateTime atualizadoEmUtc)
    {
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        RegistrarAtualizacao(atualizadoEmUtc);
    }
}
