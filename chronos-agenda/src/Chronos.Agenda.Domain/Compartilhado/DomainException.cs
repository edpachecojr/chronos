namespace Chronos.Agenda.Domain.Compartilhado;

/// <summary>Indica uma regra de domínio violada fora de um fluxo esperado.</summary>
public sealed class DomainException(string mensagem) : Exception(mensagem);
