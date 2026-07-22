using Chronos.Agenda.Domain.Compartilhado;

namespace Chronos.Agenda.Domain.Servicos;

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
        return new Servico(Guid.NewGuid(), organizacaoId, profissionalId, nome, duracao, preco, tipoAtendimento, criadoEmUtc, criadoEmUtc);
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
            throw new DomainException("O serviço requer uma organização e um profissional válidos.");
        }
    }
}
