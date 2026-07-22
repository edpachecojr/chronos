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
        PessoaAtendida pessoaAtendida,
        PeriodoAgendamento periodo,
        PrecoServico precoCobrado,
        LocalAtendimento local,
        Auditoria auditoria,
        StatusAgendamento status)
        : base(id, auditoria)
    {
        OrganizacaoId = organizacaoId;
        ProfissionalId = profissionalId;
        ServicoId = servicoId;
        PessoaAtendida = pessoaAtendida;
        Periodo = periodo;
        PrecoCobrado = precoCobrado;
        Local = local;
        Status = status;
    }

    public Guid OrganizacaoId { get; }
    public Guid ProfissionalId { get; }
    public Guid ServicoId { get; }
    public PessoaAtendida PessoaAtendida { get; private set; }
    public PeriodoAgendamento Periodo { get; private set; }
    public PrecoServico PrecoCobrado { get; private set; }
    public LocalAtendimento Local { get; private set; }
    public TipoAtendimento TipoAtendimento => Local.Tipo;
    public StatusAgendamento Status { get; private set; }

    /// <summary>Cria um novo agendamento pendente.</summary>
    /// <example><code>var agendamento = Agendamento.Criar(organizacaoId, profissionalId, servicoId, pessoaAtendida, periodo, preco, local, provedorDataHora);</code></example>
    public static Agendamento Criar(Guid organizacaoId, Guid profissionalId, Guid servicoId, PessoaAtendida pessoaAtendida, PeriodoAgendamento periodo, PrecoServico precoCobrado, LocalAtendimento local, IProvedorDataHora provedorDataHora)
    {
        ValidarReferencias(organizacaoId, profissionalId, servicoId);
        var auditoria = Auditoria.Criar(provedorDataHora);
        var agendamento = new Agendamento(
            Guid.NewGuid(),
            organizacaoId,
            profissionalId,
            servicoId,
            pessoaAtendida ?? throw new ArgumentNullException(nameof(pessoaAtendida)),
            periodo ?? throw new ArgumentNullException(nameof(periodo)),
            precoCobrado ?? throw new ArgumentNullException(nameof(precoCobrado)),
            local ?? throw new ArgumentNullException(nameof(local)),
            auditoria,
            StatusAgendamento.Pendente);
        agendamento.LancarEventoDominio(new AgendamentoCriado(agendamento.Id, organizacaoId, profissionalId, servicoId, auditoria.CriadoEmUtc));
        return agendamento;
    }

    /// <summary>Altera os dados de um agendamento que ainda não foi cancelado.</summary>
    /// <example><code>agendamento.Atualizar(pessoaAtendida, periodo, preco, local, provedorDataHora);</code></example>
    public void Atualizar(
        PessoaAtendida pessoaAtendida,
        PeriodoAgendamento periodo,
        PrecoServico precoCobrado,
        LocalAtendimento local,
        IProvedorDataHora provedorDataHora)
    {
        ExigirNaoCancelado();
        PessoaAtendida = pessoaAtendida ?? throw new ArgumentNullException(nameof(pessoaAtendida));
        Periodo = periodo ?? throw new ArgumentNullException(nameof(periodo));
        PrecoCobrado = precoCobrado ?? throw new ArgumentNullException(nameof(precoCobrado));
        Local = local ?? throw new ArgumentNullException(nameof(local));
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
        ArgumentNullException.ThrowIfNull(outro);
        return Status != StatusAgendamento.Cancelado
            && outro.Status != StatusAgendamento.Cancelado
            && ProfissionalId == outro.ProfissionalId
            && Periodo.Sobrepoe(outro.Periodo);
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
