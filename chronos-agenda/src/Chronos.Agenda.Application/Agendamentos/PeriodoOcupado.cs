using Chronos.Agenda.Domain.Agendamentos.Enums;

namespace Chronos.Agenda.Application.Agendamentos;

/// <summary>Um intervalo já ocupado por um agendamento, projetado no fuso horário da organização, para a consulta
/// somente leitura da agenda (UC07). Traz o nome do serviço contratado e da pessoa atendida para que a tela de
/// agenda não precise de uma segunda consulta por agendamento.</summary>
public sealed record PeriodoOcupado(
    Guid AgendamentoId, TimeOnly Inicio, TimeOnly Fim, StatusAgendamento Status, string NomeServico, string NomePessoaAtendida);
