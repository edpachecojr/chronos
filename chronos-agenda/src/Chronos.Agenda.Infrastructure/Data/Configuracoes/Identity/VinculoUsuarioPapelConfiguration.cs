using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Agenda.Infrastructure.Data.Configuracoes.Identity;

/// <summary>Mapeia o vínculo usuário↔papel do Identity para a tabela <c>user_roles</c>, sem o prefixo <c>AspNet</c>
/// (ADR 0001).</summary>
public sealed class VinculoUsuarioPapelConfiguration : IEntityTypeConfiguration<IdentityUserRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder)
    {
        builder.ToTable("user_roles");
    }
}
