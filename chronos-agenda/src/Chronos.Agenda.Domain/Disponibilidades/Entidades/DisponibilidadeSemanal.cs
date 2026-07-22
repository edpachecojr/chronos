using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Entidades;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Disponibilidades.EventosDominio;
using Chronos.Agenda.Domain.Disponibilidades.Exceptions;
using Chronos.Agenda.Domain.Disponibilidades.ObjetosValor;

namespace Chronos.Agenda.Domain.Disponibilidades.Entidades;

/// <summary>Representa uma janela semanal em que um profissional atende.</summary>
public sealed class DisponibilidadeSemanal : Entidade, IPertenceOrganizacao
{
    private DisponibilidadeSemanal(
        Guid id,
        Guid organizacaoId,
        Guid profissionalId,
        DayOfWeek diaDaSemana,
        JanelaHorario janela,
        Auditoria auditoria)
        : base(id, auditoria)
    {
        OrganizacaoId = organizacaoId;
        ProfissionalId = profissionalId;
        DiaDaSemana = diaDaSemana;
        Janela = janela;
    }

    public Guid OrganizacaoId { get; }
    public Guid ProfissionalId { get; }
    public DayOfWeek DiaDaSemana { get; private set; }
    public JanelaHorario Janela { get; private set; }

    /// <summary>Cria uma nova disponibilidade semanal para um profissional.</summary>
    /// <example><code>var disponibilidade = DisponibilidadeSemanal.Criar(organizacaoId, profissionalId, DayOfWeek.Monday, janela, provedorDataHora);</code></example>
    public static DisponibilidadeSemanal Criar(Guid organizacaoId, Guid profissionalId, DayOfWeek diaDaSemana, JanelaHorario janela, IProvedorDataHora provedorDataHora)
    {
        ValidarPropriedade(organizacaoId, profissionalId);
        var auditoria = Auditoria.Criar(provedorDataHora);
        var disponibilidade = new DisponibilidadeSemanal(Guid.NewGuid(), organizacaoId, profissionalId, diaDaSemana, janela, auditoria);
        disponibilidade.LancarEventoDominio(new DisponibilidadeSemanalCriada(disponibilidade.Id, organizacaoId, profissionalId, auditoria.CriadoEmUtc));
        return disponibilidade;
    }

    /// <summary>Altera o dia ou a janela de atendimento configurada.</summary>
    /// <example><code>disponibilidade.Reagendar(DayOfWeek.Monday, janela, provedorDataHora);</code></example>
    public void Reagendar(DayOfWeek diaDaSemana, JanelaHorario janela, IProvedorDataHora provedorDataHora)
    {
        DiaDaSemana = diaDaSemana;
        Janela = janela;
        Auditoria.Atualizar(provedorDataHora);
    }

    /// <summary>Informa se outra disponibilidade ocupa uma janela sobreposta do mesmo profissional no mesmo dia.</summary>
    /// <example><code>var conflita = disponibilidade.ConflitaCom(outraDisponibilidade);</code></example>
    public bool ConflitaCom(DisponibilidadeSemanal outra)
    {
        ArgumentNullException.ThrowIfNull(outra);
        return ProfissionalId == outra.ProfissionalId
            && DiaDaSemana == outra.DiaDaSemana
            && Janela.Sobrepoe(outra.Janela);
    }

    private static void ValidarPropriedade(Guid organizacaoId, Guid profissionalId)
    {
        if (organizacaoId == Guid.Empty || profissionalId == Guid.Empty)
        {
            throw new PropriedadeDisponibilidadeInvalidaException(organizacaoId, profissionalId);
        }
    }
}
