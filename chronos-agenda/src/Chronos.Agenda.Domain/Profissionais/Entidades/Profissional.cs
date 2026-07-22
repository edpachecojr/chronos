using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Entidades;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Profissionais.EventosDominio;
using Chronos.Agenda.Domain.Profissionais.Exceptions;

namespace Chronos.Agenda.Domain.Profissionais.Entidades;

/// <summary>Representa quem presta serviços dentro de uma organização.</summary>
public sealed class Profissional : Entidade, IPertenceOrganizacao
{
    private Profissional(
        Guid id,
        Guid organizacaoId,
        Nome nome,
        Auditoria auditoria)
        : base(id, auditoria)
    {
        OrganizacaoId = organizacaoId;
        Nome = nome;
    }

    public Guid OrganizacaoId { get; }
    public Nome Nome { get; private set; }

    /// <summary>Cria um novo profissional vinculado a uma organização.</summary>
    /// <example><code>var profissional = Profissional.Criar(organizacaoId, nome, provedorDataHora);</code></example>
    public static Profissional Criar(Guid organizacaoId, Nome nome, IProvedorDataHora provedorDataHora)
    {
        ValidarOrganizacao(organizacaoId);
        var auditoria = Auditoria.Criar(provedorDataHora);
        var profissional = new Profissional(Guid.NewGuid(), organizacaoId, nome, auditoria);
        profissional.LancarEventoDominio(new ProfissionalCriado(profissional.Id, organizacaoId, auditoria.CriadoEmUtc));
        return profissional;
    }

    public void Renomear(Nome nome, IProvedorDataHora provedorDataHora)
    {
        Nome = nome;
        Auditoria.Atualizar(provedorDataHora);
    }

    private static void ValidarOrganizacao(Guid organizacaoId)
    {
        if (organizacaoId == Guid.Empty)
        {
            throw new OrganizacaoProfissionalInvalidaException(organizacaoId);
        }
    }
}
