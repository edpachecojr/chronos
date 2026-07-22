using Chronos.Agenda.Domain.Agendamentos.Erros;
using Chronos.Agenda.Domain.Agendamentos.EventosDominio;
using Chronos.Agenda.Domain.Agendamentos.Exceptions;
using Chronos.Agenda.Domain.Agendamentos.Enums;
using Chronos.Agenda.Domain.Agendamentos.ObjetosValor;
using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Entidades;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Compartilhado.Resultados;
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
        string nomeServicoContratado,
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
        NomeServicoContratado = nomeServicoContratado;
        PessoaAtendida = pessoaAtendida;
        Periodo = periodo;
        PrecoCobrado = precoCobrado;
        Local = local;
        Status = status;
    }

    /// <summary>Construtor sem parâmetros usado apenas pelo EF Core para materialização (ver <see cref="Entidade"/>).</summary>
    private Agendamento()
    {
        NomeServicoContratado = null!;
        PessoaAtendida = null!;
        Periodo = null!;
        PrecoCobrado = null!;
        Local = null!;
    }

    public Guid OrganizacaoId { get; }
    public Guid ProfissionalId { get; }
    public Guid ServicoId { get; }
    public string NomeServicoContratado { get; }
    public PessoaAtendida PessoaAtendida { get; private set; }
    public PeriodoAgendamento Periodo { get; private set; }
    public PrecoServico PrecoCobrado { get; private set; }
    public LocalAtendimento Local { get; private set; }
    public TipoAtendimento TipoAtendimento => Local.Tipo;
    public StatusAgendamento Status { get; private set; }
    public TimeSpan DuracaoReservada => Periodo.Duracao;

    /// <summary>Cria um novo agendamento pendente, preservando o nome do serviço vigente na contratação (RN05).</summary>
    /// <example><code>var agendamento = Agendamento.Criar(organizacaoId, profissionalId, servicoId, servico.Nome.Valor, pessoaAtendida, periodo, preco, local, provedorDataHora);</code></example>
    public static Agendamento Criar(Guid organizacaoId, Guid profissionalId, Guid servicoId, string nomeServicoContratado, PessoaAtendida pessoaAtendida, PeriodoAgendamento periodo, PrecoServico precoCobrado, LocalAtendimento local, IProvedorDataHora provedorDataHora)
    {
        ValidarReferencias(organizacaoId, profissionalId, servicoId);
        ArgumentException.ThrowIfNullOrWhiteSpace(nomeServicoContratado);
        var auditoria = Auditoria.Criar(provedorDataHora);
        var agendamento = new Agendamento(
            Guid.NewGuid(),
            organizacaoId,
            profissionalId,
            servicoId,
            nomeServicoContratado,
            pessoaAtendida,
            periodo,
            precoCobrado,
            local,
            auditoria,
            StatusAgendamento.Pendente);
        agendamento.LancarEventoDominio(new AgendamentoCriado(agendamento.Id, organizacaoId, profissionalId, servicoId, auditoria.CriadoEmUtc));
        return agendamento;
    }

    /// <summary>Altera os dados de um agendamento que ainda não foi cancelado.</summary>
    /// <example><code>var resultado = agendamento.Atualizar(pessoaAtendida, periodo, preco, local, provedorDataHora);</code></example>
    public Resultado Atualizar(
        PessoaAtendida pessoaAtendida,
        PeriodoAgendamento periodo,
        PrecoServico precoCobrado,
        LocalAtendimento local,
        IProvedorDataHora provedorDataHora)
    {
        if (EstaCancelado())
        {
            return Resultado.Falha(AgendamentoErros.JaCancelado);
        }

        PessoaAtendida = pessoaAtendida;
        Periodo = periodo;
        PrecoCobrado = precoCobrado;
        Local = local;
        Auditoria.Atualizar(provedorDataHora);
        return Resultado.Ok();
    }

    /// <summary>Confirma um agendamento pendente.</summary>
    /// <example><code>var resultado = agendamento.Confirmar(provedorDataHora);</code></example>
    public Resultado Confirmar(IProvedorDataHora provedorDataHora)
    {
        if (Status != StatusAgendamento.Pendente)
        {
            return Resultado.Falha(AgendamentoErros.ConfirmacaoInvalida(Status));
        }

        Status = StatusAgendamento.Confirmado;
        Auditoria.Atualizar(provedorDataHora);
        return Resultado.Ok();
    }

    /// <summary>Cancela um agendamento que ainda está ativo.</summary>
    /// <example><code>var resultado = agendamento.Cancelar(provedorDataHora);</code></example>
    public Resultado Cancelar(IProvedorDataHora provedorDataHora)
    {
        if (EstaCancelado())
        {
            return Resultado.Falha(AgendamentoErros.JaCancelado);
        }

        Status = StatusAgendamento.Cancelado;
        Auditoria.Atualizar(provedorDataHora);
        return Resultado.Ok();
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

    private bool EstaCancelado()
    {
        return Status == StatusAgendamento.Cancelado;
    }

    private static void ValidarReferencias(Guid organizacaoId, Guid profissionalId, Guid servicoId)
    {
        if (organizacaoId == Guid.Empty || profissionalId == Guid.Empty || servicoId == Guid.Empty)
        {
            throw new ReferenciasAgendamentoInvalidasException(organizacaoId, profissionalId, servicoId);
        }
    }
}
