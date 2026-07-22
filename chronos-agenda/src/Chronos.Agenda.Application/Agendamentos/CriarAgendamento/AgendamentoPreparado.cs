using Chronos.Agenda.Domain.Agendamentos.ObjetosValor;
using Chronos.Agenda.Domain.Organizacoes.Entidades;
using Chronos.Agenda.Domain.Profissionais.Entidades;
using Chronos.Agenda.Domain.Servicos.Entidades;

namespace Chronos.Agenda.Application.Agendamentos.CriarAgendamento;

/// <summary>Dados já validados e prontos para criar o agendamento (RN04, RN05, RN06), antes das checagens de
/// disponibilidade (RN07) e conflito de agenda (RN02).</summary>
internal sealed record AgendamentoPreparado(
    Profissional Profissional,
    Servico Servico,
    Organizacao Organizacao,
    PessoaAtendida PessoaAtendida,
    PeriodoAgendamento Periodo,
    LocalAtendimento Local);
