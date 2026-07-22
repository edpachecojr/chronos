namespace Chronos.Agenda.Domain.Usuarios.Enums;

/// <summary>
/// Papel de um usuário dentro da organização à qual pertence. `Proprietario`
/// identifica quem responde pela organização; `Membro` é um papel genérico
/// para os demais usuários até que profissional, cliente e outros papéis
/// específicos sejam modelados (ver docs/backlog/plano-implementacao-mvp.md).
/// </summary>
public enum PapelUsuario
{
    Proprietario = 1,
    Membro = 2,
}
