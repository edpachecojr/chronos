using Chronos.Agenda.Domain.Compartilhado.Excecoes;

namespace Chronos.Agenda.Domain.Agendamentos.Excecoes;

/// <summary>Indica referências obrigatórias ausentes ao criar um agendamento.</summary>
public sealed class ReferenciasAgendamentoInvalidasException(Guid organizacaoId, Guid profissionalId, Guid servicoId)
    : DomainException($"O agendamento requer organização, profissional e serviço válidos; recebidos: organização {organizacaoId}, profissional {profissionalId}, serviço {servicoId}.");
