using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Entidades;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Organizacoes.EventosDominio;
using Chronos.Agenda.Domain.Organizacoes.ObjetosValor;

namespace Chronos.Agenda.Domain.Organizacoes.Entidades;

/// <summary>Representa o limite de propriedade e acesso do negócio no Chronos.</summary>
public sealed class Organizacao : Entidade
{
    private Organizacao(Guid id, NomeOrganizacao nome, Auditoria auditoria)
        : base(id, auditoria)
    {
        Nome = nome;
    }

    public NomeOrganizacao Nome { get; private set; }

    /// <summary>Cria uma nova organização.</summary>
    /// <example><code>var organizacao = Organizacao.Criar(nome, provedorDataHora);</code></example>
    public static Organizacao Criar(NomeOrganizacao nome, IProvedorDataHora provedorDataHora)
    {
        var auditoria = Auditoria.Criar(provedorDataHora);
        var organizacao = new Organizacao(Guid.NewGuid(), nome, auditoria);
        organizacao.LancarEventoDominio(new OrganizacaoCriada(organizacao.Id, auditoria.CriadoEmUtc));
        return organizacao;
    }

    public void Renomear(NomeOrganizacao nome, IProvedorDataHora provedorDataHora)
    {
        Nome = nome;
        Auditoria.Atualizar(provedorDataHora);
    }
}
