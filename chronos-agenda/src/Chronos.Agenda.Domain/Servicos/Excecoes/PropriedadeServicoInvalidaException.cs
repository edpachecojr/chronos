using Chronos.Agenda.Domain.Compartilhado.Excecoes;

namespace Chronos.Agenda.Domain.Servicos.Excecoes;

/// <summary>Indica referências de organização ou profissional inválidas em um serviço.</summary>
public sealed class PropriedadeServicoInvalidaException(Guid organizacaoId, Guid profissionalId)
    : DomainException($"O serviço requer uma organização e um profissional válidos; recebidos: organização {organizacaoId}, profissional {profissionalId}.");
