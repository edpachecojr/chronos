using Chronos.Agenda.Domain.Agendamentos.Enums;

namespace Chronos.Agenda.Application.Agendamentos;

/// <summary>Um intervalo já ocupado por um agendamento, projetado no fuso horário da organização, para a consulta
/// somente leitura da agenda (UC07). Traz o nome do serviço contratado e da pessoa atendida para que a tela de
/// agenda não precise de uma segunda consulta por agendamento; <see cref="ServicoId"/>,
/// <see cref="TipoPessoaAtendida"/> e <see cref="EnderecoPessoaAtendida"/> existem para que a tela de reagendar
/// (UC05) consiga pré-preencher o formulário com o estado atual, já que reagendar reenvia o registro inteiro, não
/// um patch parcial.</summary>
public sealed record PeriodoOcupado(
    Guid AgendamentoId,
    Guid ServicoId,
    TimeOnly Inicio,
    TimeOnly Fim,
    StatusAgendamento Status,
    string NomeServico,
    string NomePessoaAtendida,
    TipoPessoaAtendida TipoPessoaAtendida,
    string? EnderecoPessoaAtendida);
