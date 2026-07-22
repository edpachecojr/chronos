using Chronos.Agenda.Domain.Compartilhado.EventosDominio;
using Chronos.Agenda.Domain.Compartilhado.Excecoes;

namespace Chronos.Agenda.Domain.Compartilhado.Entidades;

/// <summary>Representa um objeto do domínio com identidade e auditoria comuns.</summary>
public abstract class Entidade
{
    private readonly List<IEventoDominio> eventosDominio = [];

    protected Entidade(Guid id, DateTime criadoEmUtc, DateTime atualizadoEmUtc)
    {
        Id = id;
        CriadoEmUtc = criadoEmUtc;
        AtualizadoEmUtc = atualizadoEmUtc;
    }

    public Guid Id { get; }
    public DateTime CriadoEmUtc { get; }
    public DateTime AtualizadoEmUtc { get; private set; }

    /// <summary>Obtém os eventos de domínio ainda não processados da entidade.</summary>
    /// <example><code>var eventos = entidade.ObterEventosDominio();</code></example>
    public IReadOnlyCollection<IEventoDominio> ObterEventosDominio()
    {
        return eventosDominio.AsReadOnly();
    }

    /// <summary>Remove os eventos de domínio que já foram processados.</summary>
    /// <example><code>entidade.LimparEventosDominio();</code></example>
    public void LimparEventosDominio()
    {
        eventosDominio.Clear();
    }

    /// <summary>Registra um fato de domínio produzido pela entidade.</summary>
    /// <example><code>LancarEventoDominio(evento);</code></example>
    protected void LancarEventoDominio(IEventoDominio eventoDominio)
    {
        eventosDominio.Add(eventoDominio);
    }

    protected static void ValidarCriacao(DateTime criadoEmUtc)
    {
        ExigirUtc(criadoEmUtc, nameof(criadoEmUtc));
    }

    protected void RegistrarAtualizacao(DateTime atualizadoEmUtc)
    {
        var instante = ExigirUtc(atualizadoEmUtc, nameof(atualizadoEmUtc));
        if (instante < CriadoEmUtc)
        {
            throw new AtualizacaoAnteriorCriacaoException(CriadoEmUtc, instante);
        }

        AtualizadoEmUtc = instante;
    }

    private static DateTime ExigirUtc(DateTime instante, string nomeParametro)
    {
        if (instante.Kind != DateTimeKind.Utc)
        {
            throw new InstanteDeveEstarEmUtcException(nomeParametro, instante.Kind);
        }

        return instante;
    }
}
