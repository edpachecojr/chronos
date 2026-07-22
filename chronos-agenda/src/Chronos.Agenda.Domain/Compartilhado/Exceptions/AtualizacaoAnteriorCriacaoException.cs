namespace Chronos.Agenda.Domain.Compartilhado.Exceptions;

/// <summary>Indica uma atualização anterior ao instante de criação da entidade.</summary>
public sealed class AtualizacaoAnteriorCriacaoException(DateTime criadoEmUtc, DateTime atualizadoEmUtc)
    : DomainException($"A atualização em {atualizadoEmUtc:O} não pode ser anterior à criação em {criadoEmUtc:O}.");
