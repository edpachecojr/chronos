using Chronos.Agenda.Domain.Compartilhado;

namespace Chronos.Agenda.Domain.Servicos;

/// <summary>Representa um serviço oferecido por um profissional da organização.</summary>
public sealed class Servico : Entidade, IPertenceOrganizacao
{
    public Servico(
        Guid id,
        Guid organizacaoId,
        Guid profissionalId,
        NomeServico nome,
        DuracaoServico duracao,
        PrecoServico preco,
        TipoAtendimento tipoAtendimento,
        DateTime criadoEmUtc)
        : base(id, criadoEmUtc)
    {
        if (organizacaoId == Guid.Empty || profissionalId == Guid.Empty)
        {
            throw new DomainException("O serviço requer uma organização e um profissional válidos.");
        }

        OrganizacaoId = organizacaoId;
        ProfissionalId = profissionalId;
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        Duracao = duracao ?? throw new ArgumentNullException(nameof(duracao));
        Preco = preco ?? throw new ArgumentNullException(nameof(preco));
        TipoAtendimento = tipoAtendimento;
    }

    public Guid OrganizacaoId { get; }
    public Guid ProfissionalId { get; }
    public NomeServico Nome { get; private set; }
    public DuracaoServico Duracao { get; private set; }
    public PrecoServico Preco { get; private set; }
    public TipoAtendimento TipoAtendimento { get; private set; }

    /// <summary>Atualiza a configuração comercial de um serviço.</summary>
    /// <example><code>servico.Atualizar(nome, duracao, preco, TipoAtendimento.Online, agoraUtc);</code></example>
    public void Atualizar(
        NomeServico nome,
        DuracaoServico duracao,
        PrecoServico preco,
        TipoAtendimento tipoAtendimento,
        DateTime atualizadoEmUtc)
    {
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        Duracao = duracao ?? throw new ArgumentNullException(nameof(duracao));
        Preco = preco ?? throw new ArgumentNullException(nameof(preco));
        TipoAtendimento = tipoAtendimento;
        RegistrarAtualizacao(atualizadoEmUtc);
    }
}
