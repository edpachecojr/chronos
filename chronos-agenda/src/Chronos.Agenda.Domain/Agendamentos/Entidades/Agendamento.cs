using Chronos.Agenda.Domain.Agendamentos.EventosDominio;
using Chronos.Agenda.Domain.Agendamentos.Exceptions;
using Chronos.Agenda.Domain.Agendamentos.Enums;
using Chronos.Agenda.Domain.Agendamentos.ObjetosValor;
using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Entidades;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
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
        Auditoria auditoria,
        StatusAgendamento status)
        : base(id, auditoria)
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
    /// <example><code>var agendamento = Agendamento.Criar(organizacaoId, profissionalId, servicoId, cliente, periodo, preco, tipo, provedorDataHora);</code></example>
    public static Agendamento Criar(Guid organizacaoId, Guid profissionalId, Guid servicoId, NomeCliente cliente, PeriodoAgendamento periodo, PrecoServico precoCobrado, TipoAtendimento tipoAtendimento, IProvedorDataHora provedorDataHora)
    {
        ValidarReferencias(organizacaoId, profissionalId, servicoId);
        var auditoria = Auditoria.Criar(provedorDataHora);
        var agendamento = new Agendamento(Guid.NewGuid(), organizacaoId, profissionalId, servicoId, cliente, periodo, precoCobrado, tipoAtendimento, auditoria, StatusAgendamento.Pendente);
        agendamento.LancarEventoDominio(new AgendamentoCriado(agendamento.Id, organizacaoId, profissionalId, servicoId, auditoria.CriadoEmUtc));
        return agendamento;
    }

    /// <summary>Altera os dados de um agendamento que ainda não foi cancelado.</summary>
    /// <example><code>agendamento.Atualizar(cliente, periodo, preco, tipo, provedorDataHora);</code></example>
    public void Atualizar(
        NomeCliente cliente,
        PeriodoAgendamento periodo,
        PrecoServico precoCobrado,
        TipoAtendimento tipoAtendimento,
        IProvedorDataHora provedorDataHora)
    {
        ExigirNaoCancelado();
        Cliente = cliente;
        Periodo = periodo;
        PrecoCobrado = precoCobrado;
        TipoAtendimento = tipoAtendimento;
        Auditoria.Atualizar(provedorDataHora);
    }

    /// <summary>Confirma um agendamento pendente.</summary>
    /// <example><code>agendamento.Confirmar(provedorDataHora);</code></example>
    public void Confirmar(IProvedorDataHora provedorDataHora)
    {
        if (Status != StatusAgendamento.Pendente)
        {
            throw new ConfirmacaoAgendamentoInvalidaException(Status);
        }

        Status = StatusAgendamento.Confirmado;
        Auditoria.Atualizar(provedorDataHora);
    }

    /// <summary>Cancela um agendamento que ainda está ativo.</summary>
    /// <example><code>agendamento.Cancelar(provedorDataHora);</code></example>
    public void Cancelar(IProvedorDataHora provedorDataHora)
    {
        ExigirNaoCancelado();
        Status = StatusAgendamento.Cancelado;
        Auditoria.Atualizar(provedorDataHora);
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
