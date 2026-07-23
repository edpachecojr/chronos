namespace Chronos.Agenda.Application.Profissionais.ListarProfissionais;

/// <summary>Projeção somente leitura de um profissional vinculado à organização corrente.</summary>
public sealed record ProfissionalResumo(Guid ProfissionalId, string Nome);
