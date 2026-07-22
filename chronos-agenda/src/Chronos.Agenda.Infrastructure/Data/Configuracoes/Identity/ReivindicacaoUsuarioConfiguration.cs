using Chronos.Agenda.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Agenda.Infrastructure.Data.Configuracoes.Identity;

/// <summary>Mapeia as claims de usuário do Identity para a tabela <c>user_claims</c>, sem o prefixo <c>AspNet</c>
/// (ADR 0001).</summary>
public sealed class ReivindicacaoUsuarioConfiguration : IEntityTypeConfiguration<IdentityUserClaim<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserClaim<Guid>> builder)
    {
        builder.ToTable("user_claims");
        builder.Property(c => c.ClaimType).HasMaxLength(256);
        builder.Property(c => c.ClaimValue).HasMaxLength(256);

        builder.HasOne<UsuarioIdentity>()
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .HasConstraintName("fk_user_claims_usuarios_user_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
