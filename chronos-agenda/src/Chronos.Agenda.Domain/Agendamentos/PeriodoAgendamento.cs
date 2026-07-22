using Chronos.Agenda.Domain.Compartilhado;
using Chronos.Agenda.Domain.Servicos;

namespace Chronos.Agenda.Domain.Agendamentos;

/// <summary>Representa o intervalo UTC ocupado por um agendamento.</summary>
public sealed record PeriodoAgendamento
{
    public PeriodoAgendamento(DateTime inicioUtc, DuracaoServico duracao)
    {
        if (inicioUtc.Kind != DateTimeKind.Utc)
        {
            throw new DomainException("O início do agendamento deve estar em UTC.");
        }

        InicioUtc = inicioUtc;
        Duracao = duracao ?? throw new ArgumentNullException(nameof(duracao));
    }

    public DateTime InicioUtc { get; }
    public DuracaoServico Duracao { get; }
    public DateTime FimUtc => InicioUtc.Add(Duracao.Valor);

    /// <summary>Informa se este período tem algum instante em comum com outro.</summary>
    /// <example><code>var conflita = periodo.Sobrepoe(outroPeriodo);</code></example>
    public bool Sobrepoe(PeriodoAgendamento outro)
    {
        ArgumentNullException.ThrowIfNull(outro);
        return InicioUtc < outro.FimUtc && outro.InicioUtc < FimUtc;
    }
}
