using Chronos.Agenda.Domain.Agendamentos.Excecoes;
using Chronos.Agenda.Domain.Servicos.ObjetosValor;

namespace Chronos.Agenda.Domain.Agendamentos.ObjetosValor;

/// <summary>Representa o intervalo UTC ocupado por um agendamento.</summary>
public sealed record PeriodoAgendamento
{
    public PeriodoAgendamento(DateTime inicioUtc, DuracaoServico duracao)
    {
        if (inicioUtc.Kind != DateTimeKind.Utc)
        {
            throw new InicioAgendamentoNaoEstaEmUtcException(inicioUtc.Kind);
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
