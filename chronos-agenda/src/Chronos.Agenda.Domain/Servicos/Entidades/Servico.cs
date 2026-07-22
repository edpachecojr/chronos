using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Entidades;
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
        DateTime criadoEmUtc,
        DateTime atualizadoEmUtc)
        : base(id, criadoEmUtc, atualizadoEmUtc)
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
    /// <example><code>var servico = Servico.Criar(organizacaoId, profissionalId, nome, duracao, preco, tipo, agoraUtc);</code></example>
    public static Servico Criar(Guid organizacaoId, Guid profissionalId, NomeServico nome, DuracaoServico duracao, PrecoServico preco, TipoAtendimento tipoAtendimento, DateTime criadoEmUtc)
    {
        ValidarCriacao(criadoEmUtc);
        ValidarPropriedade(organizacaoId, profissionalId);
        var servico = new Servico(Guid.NewGuid(), organizacaoId, profissionalId, nome, duracao, preco, tipoAtendimento, criadoEmUtc, criadoEmUtc);
        servico.LancarEventoDominio(new ServicoCriado(servico.Id, organizacaoId, profissionalId, criadoEmUtc));
        return servico;
    }

    /// <summary>Reconstitui um serviço previamente persistido, sem executar regras de criação.</summary>
    /// <example><code>var servico = Servico.Reidratar(id, organizacaoId, profissionalId, nome, duracao, preco, tipo, criadoEmUtc, atualizadoEmUtc);</code></example>
    public static Servico Reidratar(Guid id, Guid organizacaoId, Guid profissionalId, NomeServico nome, DuracaoServico duracao, PrecoServico preco, TipoAtendimento tipoAtendimento, DateTime criadoEmUtc, DateTime atualizadoEmUtc)
    {
        return new Servico(id, organizacaoId, profissionalId, nome, duracao, preco, tipoAtendimento, criadoEmUtc, atualizadoEmUtc);
    }

    /// <summary>Atualiza a configuração comercial de um serviço.</summary>
    /// <example><code>servico.Atualizar(nome, duracao, preco, TipoAtendimento.Online, agoraUtc);</code></example>
    public void Atualizar(
        NomeServico nome,
        DuracaoServico duracao,
        PrecoServico preco,
        TipoAtendimento tipoAtendimento,
        DateTime atualizadoEmUtc)
    {
        Nome = nome;
        Duracao = duracao;
        Preco = preco;
        TipoAtendimento = tipoAtendimento;
        RegistrarAtualizacao(atualizadoEmUtc);
    }

    private static void ValidarPropriedade(Guid organizacaoId, Guid profissionalId)
    {
        if (organizacaoId == Guid.Empty || profissionalId == Guid.Empty)
        {
            throw new PropriedadeServicoInvalidaException(organizacaoId, profissionalId);
        }
    }
}
