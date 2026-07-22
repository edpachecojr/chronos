using Chronos.Agenda.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Agenda.Infrastructure.Data.Configuracoes.Identity;

/// <summary>Mapeia os logins externos de usuário do Identity para a tabela <c>user_logins</c>, sem o prefixo
/// <c>AspNet</c> (ADR 0001).</summary>
public sealed class LoginUsuarioConfiguration : IEntityTypeConfiguration<IdentityUserLogin<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserLogin<Guid>> builder)
    {
        builder.ToTable("user_logins");
        builder.Property(l => l.LoginProvider).HasMaxLength(128);
        builder.Property(l => l.ProviderKey).HasMaxLength(128);
        builder.Property(l => l.ProviderDisplayName).HasMaxLength(128);

        builder.HasOne<UsuarioIdentity>()
            .WithMany()
            .HasForeignKey(l => l.UserId)
            .HasConstraintName("fk_user_logins_usuarios_user_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
