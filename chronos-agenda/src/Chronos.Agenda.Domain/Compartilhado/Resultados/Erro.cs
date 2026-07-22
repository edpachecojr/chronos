namespace Chronos.Agenda.Domain.Compartilhado.Resultados;

/// <summary>Representa um erro esperado de uma operação de domínio, identificado por um código estável.</summary>
public sealed record Erro(string Codigo, string Mensagem);
