using Chronos.Agenda.Domain.Compartilhado.Excecoes;

namespace Chronos.Agenda.Domain.Disponibilidades.Excecoes;

/// <summary>Indica referências de organização ou profissional inválidas em uma disponibilidade.</summary>
public sealed class PropriedadeDisponibilidadeInvalidaException(Guid organizacaoId, Guid profissionalId)
    : DomainException($"A disponibilidade requer uma organização e um profissional válidos; recebidos: organização {organizacaoId}, profissional {profissionalId}.");
