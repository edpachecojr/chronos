namespace Chronos.Agenda.Domain.Compartilhado.Excecoes;

/// <summary>Indica que um instante recebido não está no fuso horário UTC.</summary>
public sealed class InstanteDeveEstarEmUtcException(string nomeParametro, DateTimeKind tipo)
    : DomainException($"{nomeParametro} deve estar em UTC; tipo recebido: {tipo}.");
