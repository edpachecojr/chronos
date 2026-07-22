using Chronos.Agenda.Domain.Compartilhado.EventosDominio;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;

namespace Chronos.Agenda.Domain.Compartilhado.Entidades;

/// <summary>Representa um objeto do domínio com identidade, auditoria e eventos comuns.</summary>
public abstract class Entidade : IEquatable<Entidade>
{
    private readonly List<IEventoDominio> eventosDominio = [];

    protected Entidade(Guid id, Auditoria auditoria)
    {
        Id = id;
        Auditoria = auditoria;
    }

    /// <summary>Construtor sem parâmetros usado apenas pelo EF Core para materializar a entidade a partir do
    /// banco: objetos de valor do próprio agregado (ex.: <see cref="Auditoria"/>) não podem ser vinculados a
    /// parâmetros de construtor pelo EF Core, que preenche os campos diretamente logo após a construção.</summary>
    protected Entidade()
    {
        Id = default;
        Auditoria = null!;
    }

    public Guid Id { get; }
    public Auditoria Auditoria { get; }

    /// <summary>Compara esta entidade com outra por sua identidade.</summary>
    /// <example><code>var saoIguais = primeira.Equals(segunda);</code></example>
    public bool Equals(Entidade? outra)
    {
        return outra is not null && Id == outra.Id;
    }

    /// <inheritdoc />
    public override bool Equals(object? objeto)
    {
        return objeto is Entidade outra && Equals(outra);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

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

}
