namespace Chronos.Agenda.Domain.Compartilhado.Excecoes;

/// <summary>Indica uma regra de domínio violada fora de um fluxo esperado.</summary>
public abstract class DomainException(string mensagem) : Exception(mensagem);
