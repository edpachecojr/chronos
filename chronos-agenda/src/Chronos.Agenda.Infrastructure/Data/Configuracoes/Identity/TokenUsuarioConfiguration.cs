using Chronos.Agenda.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Agenda.Infrastructure.Data.Configuracoes.Identity;

/// <summary>Mapeia os tokens de usuário do Identity (ex.: bearer tokens, tokens de redefinição de senha) para a
/// tabela <c>user_tokens</c>, sem o prefixo <c>AspNet</c> (ADR 0001).</summary>
public sealed class TokenUsuarioConfiguration : IEntityTypeConfiguration<IdentityUserToken<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserToken<Guid>> builder)
    {
        builder.ToTable("user_tokens");
        builder.Property(t => t.LoginProvider).HasMaxLength(128);
        builder.Property(t => t.Name).HasMaxLength(128);
        builder.Property(t => t.Value).HasMaxLength(1024);

        builder.HasOne<UsuarioIdentity>()
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .HasConstraintName("fk_user_tokens_usuarios_user_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
