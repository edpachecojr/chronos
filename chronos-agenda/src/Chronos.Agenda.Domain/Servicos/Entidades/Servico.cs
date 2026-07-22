using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Entidades;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Servicos.Enums;
using Chronos.Agenda.Domain.Servicos.EventosDominio;
using Chronos.Agenda.Domain.Servicos.Exceptions;
using Chronos.Agenda.Domain.Servicos.ObjetosValor;

namespace Chronos.Agenda.Domain.Servicos.Entidades;

/// <summary>Representa um serviço oferecido por um profissional da organização.</summary>
public sealed class Servico : Entidade, IPertenceOrganizacao
{
    private Servico(
        Guid id,
        Guid organizacaoId,
        Guid profissionalId,
        NomeServico nome,
        DuracaoServico duracao,
        PrecoServico preco,
        TipoAtendimento tipoAtendimento,
        Auditoria auditoria)
        : base(id, auditoria)
    {
        OrganizacaoId = organizacaoId;
        ProfissionalId = profissionalId;
        Nome = nome;
        Duracao = duracao;
        Preco = preco;
        TipoAtendimento = tipoAtendimento;
    }

    public Guid OrganizacaoId { get; }
    public Guid ProfissionalId { get; }
    public NomeServico Nome { get; private set; }
    public DuracaoServico Duracao { get; private set; }
    public PrecoServico Preco { get; private set; }
    public TipoAtendimento TipoAtendimento { get; private set; }

    /// <summary>Cria um novo serviço oferecido por um profissional.</summary>
    /// <example><code>var servico = Servico.Criar(organizacaoId, profissionalId, nome, duracao, preco, tipo, provedorDataHora);</code></example>
    public static Servico Criar(Guid organizacaoId, Guid profissionalId, NomeServico nome, DuracaoServico duracao, PrecoServico preco, TipoAtendimento tipoAtendimento, IProvedorDataHora provedorDataHora)
    {
        ValidarPropriedade(organizacaoId, profissionalId);
        var auditoria = Auditoria.Criar(provedorDataHora);
        var servico = new Servico(Guid.NewGuid(), organizacaoId, profissionalId, nome, duracao, preco, tipoAtendimento, auditoria);
        servico.LancarEventoDominio(new ServicoCriado(servico.Id, organizacaoId, profissionalId, auditoria.CriadoEmUtc));
        return servico;
    }

    /// <summary>Atualiza a configuração comercial de um serviço.</summary>
    /// <example><code>servico.Atualizar(nome, duracao, preco, TipoAtendimento.Online, provedorDataHora);</code></example>
    public void Atualizar(
        NomeServico nome,
        DuracaoServico duracao,
        PrecoServico preco,
        TipoAtendimento tipoAtendimento,
        IProvedorDataHora provedorDataHora)
    {
        Nome = nome;
        Duracao = duracao;
        Preco = preco;
        TipoAtendimento = tipoAtendimento;
        Auditoria.Atualizar(provedorDataHora);
    }

    private static void ValidarPropriedade(Guid organizacaoId, Guid profissionalId)
    {
        if (organizacaoId == Guid.Empty || profissionalId == Guid.Empty)
        {
            throw new PropriedadeServicoInvalidaException(organizacaoId, profissionalId);
        }
    }
}
