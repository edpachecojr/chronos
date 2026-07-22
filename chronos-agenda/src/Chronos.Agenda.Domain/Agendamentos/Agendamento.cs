using Chronos.Agenda.Domain.Compartilhado;
using Chronos.Agenda.Domain.Servicos;

namespace Chronos.Agenda.Domain.Agendamentos;

/// <summary>Representa a reserva de um serviço na agenda de um profissional.</summary>
public sealed class Agendamento : Entidade, IPertenceOrganizacao
{
    public Agendamento(
        Guid id,
        Guid organizacaoId,
        Guid profissionalId,
        Guid servicoId,
        NomeCliente cliente,
        PeriodoAgendamento periodo,
        PrecoServico precoCobrado,
        TipoAtendimento tipoAtendimento,
        DateTime criadoEmUtc)
        : base(id, criadoEmUtc)
    {
        ValidarReferencias(organizacaoId, profissionalId, servicoId);
        OrganizacaoId = organizacaoId;
        ProfissionalId = profissionalId;
        ServicoId = servicoId;
        Cliente = cliente ?? throw new ArgumentNullException(nameof(cliente));
        Periodo = periodo ?? throw new ArgumentNullException(nameof(periodo));
        PrecoCobrado = precoCobrado ?? throw new ArgumentNullException(nameof(precoCobrado));
        TipoAtendimento = tipoAtendimento;
        Status = StatusAgendamento.Pendente;
    }

    public Guid OrganizacaoId { get; }
    public Guid ProfissionalId { get; }
    public Guid ServicoId { get; }
    public NomeCliente Cliente { get; private set; }
    public PeriodoAgendamento Periodo { get; private set; }
    public PrecoServico PrecoCobrado { get; private set; }
    public TipoAtendimento TipoAtendimento { get; private set; }
    public StatusAgendamento Status { get; private set; }

    /// <summary>Altera os dados de um agendamento que ainda não foi cancelado.</summary>
    /// <example><code>agendamento.Atualizar(cliente, periodo, preco, tipo, agoraUtc);</code></example>
    public void Atualizar(
        NomeCliente cliente,
        PeriodoAgendamento periodo,
        PrecoServico precoCobrado,
        TipoAtendimento tipoAtendimento,
        DateTime atualizadoEmUtc)
    {
        ExigirNaoCancelado();
        Cliente = cliente ?? throw new ArgumentNullException(nameof(cliente));
        Periodo = periodo ?? throw new ArgumentNullException(nameof(periodo));
        PrecoCobrado = precoCobrado ?? throw new ArgumentNullException(nameof(precoCobrado));
        TipoAtendimento = tipoAtendimento;
        RegistrarAtualizacao(atualizadoEmUtc);
    }

    /// <summary>Confirma um agendamento pendente.</summary>
    /// <example><code>agendamento.Confirmar(agoraUtc);</code></example>
    public void Confirmar(DateTime atualizadoEmUtc)
    {
        if (Status != StatusAgendamento.Pendente)
        {
            throw new DomainException($"Apenas agendamentos pendentes podem ser confirmados; estado atual: {Status}.");
        }

        Status = StatusAgendamento.Confirmado;
        RegistrarAtualizacao(atualizadoEmUtc);
    }

    /// <summary>Cancela um agendamento que ainda está ativo.</summary>
    /// <example><code>agendamento.Cancelar(agoraUtc);</code></example>
    public void Cancelar(DateTime atualizadoEmUtc)
    {
        ExigirNaoCancelado();
        Status = StatusAgendamento.Cancelado;
        RegistrarAtualizacao(atualizadoEmUtc);
    }

    /// <summary>Informa se outro agendamento ocupa a agenda deste profissional no mesmo intervalo.</summary>
    /// <example><code>var conflita = agendamento.ConflitaCom(outroAgendamento);</code></example>
    public bool ConflitaCom(Agendamento outro)
    {
        ArgumentNullException.ThrowIfNull(outro);
        return ProfissionalId == outro.ProfissionalId && Periodo.Sobrepoe(outro.Periodo);
    }

    private void ExigirNaoCancelado()
    {
        if (Status == StatusAgendamento.Cancelado)
        {
            throw new DomainException("Um agendamento cancelado não pode ser alterado.");
        }
    }

    private static void ValidarReferencias(Guid organizacaoId, Guid profissionalId, Guid servicoId)
    {
        if (organizacaoId == Guid.Empty || profissionalId == Guid.Empty || servicoId == Guid.Empty)
        {
            throw new DomainException("O agendamento requer organização, profissional e serviço válidos.");
        }
    }
}
