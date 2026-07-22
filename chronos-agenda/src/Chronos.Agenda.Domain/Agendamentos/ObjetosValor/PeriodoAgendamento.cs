using Chronos.Agenda.Domain.Agendamentos.Exceptions;
using Chronos.Agenda.Domain.Servicos.ObjetosValor;

namespace Chronos.Agenda.Domain.Agendamentos.ObjetosValor;

/// <summary>Representa o intervalo UTC ocupado por um agendamento. A camada de aplicação é responsável por
/// garantir que os instantes recebidos já estejam em UTC antes de chegar ao domínio.</summary>
public sealed record PeriodoAgendamento
{
    public PeriodoAgendamento(DateTime inicioUtc, DateTime fimUtc)
    {
        ExigirFimPosteriorAoInicio(inicioUtc, fimUtc);
        InicioUtc = inicioUtc;
        FimUtc = fimUtc;
    }

    public DateTime InicioUtc { get; }
    public DateTime FimUtc { get; }
    public TimeSpan Duracao => FimUtc - InicioUtc;
    public double DuracaoEmMinutos => Duracao.TotalMinutes;

    /// <summary>Calcula o fim a partir do início e da duração vigente do serviço (RN05).</summary>
    /// <example><code>var periodo = PeriodoAgendamento.APartirDaDuracao(inicioUtc, servico.Duracao);</code></example>
    public static PeriodoAgendamento APartirDaDuracao(DateTime inicioUtc, DuracaoServico duracao)
    {
        ArgumentNullException.ThrowIfNull(duracao);
        return new PeriodoAgendamento(inicioUtc, inicioUtc.Add(duracao.Valor));
    }

    /// <summary>Informa se este período tem algum instante em comum com outro.</summary>
    /// <example><code>var conflita = periodo.Sobrepoe(outroPeriodo);</code></example>
    public bool Sobrepoe(PeriodoAgendamento outro)
    {
        ArgumentNullException.ThrowIfNull(outro);
        return InicioUtc < outro.FimUtc && outro.InicioUtc < FimUtc;
    }

    private static void ExigirFimPosteriorAoInicio(DateTime inicioUtc, DateTime fimUtc)
    {
        if (fimUtc <= inicioUtc)
        {
            throw new FimAgendamentoInvalidoException(inicioUtc, fimUtc);
        }
    }
}
