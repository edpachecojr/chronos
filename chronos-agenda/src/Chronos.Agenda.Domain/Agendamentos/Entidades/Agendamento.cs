using Chronos.Agenda.Domain.Agendamentos.EventosDominio;
using Chronos.Agenda.Domain.Agendamentos.Exceptions;
using Chronos.Agenda.Domain.Agendamentos.Enums;
using Chronos.Agenda.Domain.Agendamentos.ObjetosValor;
using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Entidades;
using Chronos.Agenda.Domain.Servicos.Enums;
using Chronos.Agenda.Domain.Servicos.ObjetosValor;

namespace Chronos.Agenda.Domain.Agendamentos.Entidades;

/// <summary>Representa a reserva de um serviço na agenda de um profissional.</summary>
public sealed class Agendamento : Entidade, IPertenceOrganizacao
{
    private Agendamento(
        Guid id,
        Guid organizacaoId,
        Guid profissionalId,
        Guid servicoId,
        NomeCliente cliente,
        PeriodoAgendamento periodo,
        PrecoServico precoCobrado,
        TipoAtendimento tipoAtendimento,
        DateTime criadoEmUtc,
        DateTime atualizadoEmUtc,
        StatusAgendamento status)
        : base(id, criadoEmUtc, atualizadoEmUtc)
    {
        OrganizacaoId = organizacaoId;
        ProfissionalId = profissionalId;
        ServicoId = servicoId;
        Cliente = cliente;
        Periodo = periodo;
        PrecoCobrado = precoCobrado;
        TipoAtendimento = tipoAtendimento;
        Status = status;
    }

    public Guid OrganizacaoId { get; }
    public Guid ProfissionalId { get; }
    public Guid ServicoId { get; }
    public NomeCliente Cliente { get; private set; }
    public PeriodoAgendamento Periodo { get; private set; }
    public PrecoServico PrecoCobrado { get; private set; }
    public TipoAtendimento TipoAtendimento { get; private set; }
    public StatusAgendamento Status { get; private set; }

    /// <summary>Cria um novo agendamento pendente.</summary>
    /// <example><code>var agendamento = Agendamento.Criar(organizacaoId, profissionalId, servicoId, cliente, periodo, preco, tipo, agoraUtc);</code></example>
    public static Agendamento Criar(Guid organizacaoId, Guid profissionalId, Guid servicoId, NomeCliente cliente, PeriodoAgendamento periodo, PrecoServico precoCobrado, TipoAtendimento tipoAtendimento, DateTime criadoEmUtc)
    {
        ValidarCriacao(criadoEmUtc);
        ValidarReferencias(organizacaoId, profissionalId, servicoId);
        var agendamento = new Agendamento(Guid.NewGuid(), organizacaoId, profissionalId, servicoId, cliente, periodo, precoCobrado, tipoAtendimento, criadoEmUtc, criadoEmUtc, StatusAgendamento.Pendente);
        agendamento.LancarEventoDominio(new AgendamentoCriado(agendamento.Id, organizacaoId, profissionalId, servicoId, criadoEmUtc));
        return agendamento;
    }

    /// <summary>Reconstitui um agendamento previamente persistido, sem executar regras de criação.</summary>
    /// <example><code>var agendamento = Agendamento.Reidratar(id, organizacaoId, profissionalId, servicoId, cliente, periodo, preco, tipo, criadoEmUtc, atualizadoEmUtc, status);</code></example>
    public static Agendamento Reidratar(Guid id, Guid organizacaoId, Guid profissionalId, Guid servicoId, NomeCliente cliente, PeriodoAgendamento periodo, PrecoServico precoCobrado, TipoAtendimento tipoAtendimento, DateTime criadoEmUtc, DateTime atualizadoEmUtc, StatusAgendamento status)
    {
        return new Agendamento(id, organizacaoId, profissionalId, servicoId, cliente, periodo, precoCobrado, tipoAtendimento, criadoEmUtc, atualizadoEmUtc, status);
    }

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
        Cliente = cliente;
        Periodo = periodo;
        PrecoCobrado = precoCobrado;
        TipoAtendimento = tipoAtendimento;
        RegistrarAtualizacao(atualizadoEmUtc);
    }

    /// <summary>Confirma um agendamento pendente.</summary>
    /// <example><code>agendamento.Confirmar(agoraUtc);</code></example>
    public void Confirmar(DateTime atualizadoEmUtc)
    {
        if (Status != StatusAgendamento.Pendente)
        {
            throw new ConfirmacaoAgendamentoInvalidaException(Status);
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
        return ProfissionalId == outro.ProfissionalId && Periodo.Sobrepoe(outro.Periodo);
    }

    private void ExigirNaoCancelado()
    {
        if (Status == StatusAgendamento.Cancelado)
        {
            throw new AgendamentoCanceladoException();
        }
    }

    private static void ValidarReferencias(Guid organizacaoId, Guid profissionalId, Guid servicoId)
    {
        if (organizacaoId == Guid.Empty || profissionalId == Guid.Empty || servicoId == Guid.Empty)
        {
            throw new ReferenciasAgendamentoInvalidasException(organizacaoId, profissionalId, servicoId);
        }
    }
}
