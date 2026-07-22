using Chronos.Agenda.Domain.Compartilhado;

namespace Chronos.Agenda.Domain.Disponibilidades;

/// <summary>Representa uma janela semanal em que um profissional atende.</summary>
public sealed class DisponibilidadeSemanal : Entidade, IPertenceOrganizacao
{
    public DisponibilidadeSemanal(
        Guid id,
        Guid organizacaoId,
        Guid profissionalId,
        DayOfWeek diaDaSemana,
        JanelaHorario janela,
        DateTime criadoEmUtc)
        : base(id, criadoEmUtc)
    {
        ValidarPropriedade(organizacaoId, profissionalId);
        OrganizacaoId = organizacaoId;
        ProfissionalId = profissionalId;
        DiaDaSemana = diaDaSemana;
        Janela = janela ?? throw new ArgumentNullException(nameof(janela));
    }

    public Guid OrganizacaoId { get; }
    public Guid ProfissionalId { get; }
    public DayOfWeek DiaDaSemana { get; private set; }
    public JanelaHorario Janela { get; private set; }

    /// <summary>Altera o dia ou a janela de atendimento configurada.</summary>
    /// <example><code>disponibilidade.Reagendar(DayOfWeek.Monday, janela, agoraUtc);</code></example>
    public void Reagendar(DayOfWeek diaDaSemana, JanelaHorario janela, DateTime atualizadoEmUtc)
    {
        DiaDaSemana = diaDaSemana;
        Janela = janela ?? throw new ArgumentNullException(nameof(janela));
        RegistrarAtualizacao(atualizadoEmUtc);
    }

    private static void ValidarPropriedade(Guid organizacaoId, Guid profissionalId)
    {
        if (organizacaoId == Guid.Empty || profissionalId == Guid.Empty)
        {
            throw new DomainException("A disponibilidade requer uma organização e um profissional válidos.");
        }
    }
}
