using Microsoft.AspNetCore.Identity;

namespace Chronos.Agenda.Infrastructure.Identity;

/// <summary>Representa o usuário autenticado no ASP.NET Core Identity (ADR 0001, ADR 0003). Não é um conceito de
/// domínio: identifica apenas quem acessa o Chronos, não um papel de negócio.</summary>
public sealed class UsuarioIdentity : IdentityUser<Guid>
{
}
