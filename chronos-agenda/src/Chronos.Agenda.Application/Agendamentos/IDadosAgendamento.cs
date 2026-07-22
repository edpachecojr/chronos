using Chronos.Agenda.Domain.Agendamentos.Enums;

namespace Chronos.Agenda.Application.Agendamentos;

/// <summary>Dados de entrada comuns para preparar um agendamento (RN04-RN07): referências, pessoa atendida,
/// horário de início e endereço, quando aplicável. Implementado pelos comandos de criar e reagendar (UC04, UC05),
/// que compartilham a mesma preparação e validação via <see cref="AgendamentoPreparador"/>.</summary>
internal interface IDadosAgendamento
{
    Guid ProfissionalId { get; }
    Guid ServicoId { get; }
    string NomePessoaAtendida { get; }
    TipoPessoaAtendida TipoPessoaAtendida { get; }
    DateTimeOffset Inicio { get; }
    string? EnderecoPessoaAtendida { get; }
}
