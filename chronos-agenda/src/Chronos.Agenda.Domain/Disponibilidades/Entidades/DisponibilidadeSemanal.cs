using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Entidades;
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
        DateTime criadoEmUtc,
        DateTime atualizadoEmUtc)
        : base(id, criadoEmUtc, atualizadoEmUtc)
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
    /// <example><code>var disponibilidade = DisponibilidadeSemanal.Criar(organizacaoId, profissionalId, DayOfWeek.Monday, janela, agoraUtc);</code></example>
    public static DisponibilidadeSemanal Criar(Guid organizacaoId, Guid profissionalId, DayOfWeek diaDaSemana, JanelaHorario janela, DateTime criadoEmUtc)
    {
        ValidarCriacao(criadoEmUtc);
        ValidarPropriedade(organizacaoId, profissionalId);
        var disponibilidade = new DisponibilidadeSemanal(Guid.NewGuid(), organizacaoId, profissionalId, diaDaSemana, janela, criadoEmUtc, criadoEmUtc);
        disponibilidade.LancarEventoDominio(new DisponibilidadeSemanalCriada(disponibilidade.Id, organizacaoId, profissionalId, criadoEmUtc));
        return disponibilidade;
    }

    /// <summary>Reconstitui uma disponibilidade previamente persistida, sem executar regras de criação.</summary>
    /// <example><code>var disponibilidade = DisponibilidadeSemanal.Reidratar(id, organizacaoId, profissionalId, DayOfWeek.Monday, janela, criadoEmUtc, atualizadoEmUtc);</code></example>
    public static DisponibilidadeSemanal Reidratar(Guid id, Guid organizacaoId, Guid profissionalId, DayOfWeek diaDaSemana, JanelaHorario janela, DateTime criadoEmUtc, DateTime atualizadoEmUtc)
    {
        return new DisponibilidadeSemanal(id, organizacaoId, profissionalId, diaDaSemana, janela, criadoEmUtc, atualizadoEmUtc);
    }

    /// <summary>Altera o dia ou a janela de atendimento configurada.</summary>
    /// <example><code>disponibilidade.Reagendar(DayOfWeek.Monday, janela, agoraUtc);</code></example>
    public void Reagendar(DayOfWeek diaDaSemana, JanelaHorario janela, DateTime atualizadoEmUtc)
    {
        DiaDaSemana = diaDaSemana;
        Janela = janela;
        RegistrarAtualizacao(atualizadoEmUtc);
    }

    private static void ValidarPropriedade(Guid organizacaoId, Guid profissionalId)
    {
        if (organizacaoId == Guid.Empty || profissionalId == Guid.Empty)
        {
            throw new PropriedadeDisponibilidadeInvalidaException(organizacaoId, profissionalId);
        }
    }
}
