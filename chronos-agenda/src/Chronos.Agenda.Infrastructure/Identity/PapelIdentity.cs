using Microsoft.AspNetCore.Identity;

namespace Chronos.Agenda.Infrastructure.Identity;

/// <summary>Representa um papel de autorização do ASP.NET Core Identity. Distinto do vínculo usuário↔organização
/// (<c>membros_organizacao</c>, ADR 0003), que resolve o tenant corrente, não a autorização técnica do Identity.</summary>
public sealed class PapelIdentity : IdentityRole<Guid>
{
}
