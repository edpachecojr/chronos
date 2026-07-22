using Chronos.Agenda.Domain.Agendamentos.Enums;
using Chronos.Agenda.Domain.Compartilhado.Resultados;
using Chronos.Agenda.Domain.Servicos.Enums;

namespace Chronos.Agenda.Domain.Agendamentos.Erros;

/// <summary>Catálogo de erros esperados nas operações de agendamento.</summary>
public static class AgendamentoErros
{
    /// <summary>O local do atendimento não corresponde à modalidade contratada do serviço (RN06).</summary>
    public static Erro LocalIncompativel(TipoAtendimento tipoAtendimentoServico, TipoAtendimento tipoLocal) => new(
        "Agendamento.LocalIncompativel",
        $"O local do atendimento ({tipoLocal}) não é compatível com a modalidade do serviço ({tipoAtendimentoServico}).");

    /// <summary>Apenas agendamentos pendentes podem ser confirmados.</summary>
    public static Erro ConfirmacaoInvalida(StatusAgendamento status) => new(
        "Agendamento.ConfirmacaoInvalida",
        $"Apenas agendamentos pendentes podem ser confirmados; estado atual: {status}.");

    /// <summary>Um agendamento cancelado não pode ser confirmado, cancelado novamente ou alterado.</summary>
    public static readonly Erro JaCancelado = new(
        "Agendamento.JaCancelado",
        "Um agendamento cancelado não pode ser alterado.");
}
