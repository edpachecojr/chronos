namespace Chronos.Agenda.Application.Compartilhado;

/// <summary>
/// Papel de um usuário no vínculo com a organização (ADR 0003). Não é um
/// conceito de domínio — "proprietário" é uma questão de autorização, não
/// uma invariante de negócio — por isso vive em Application, ao lado de
/// <see cref="Contratos.IMembroOrganizacaoRepositorio"/>, que é quem o
/// persiste.
/// </summary>
public enum PapelMembroOrganizacao
{
    /// <summary>Criou a organização no onboarding (UC01) e a administra.</summary>
    Proprietario,

    /// <summary>Pertence à organização sem administrá-la.</summary>
    Membro,
}
