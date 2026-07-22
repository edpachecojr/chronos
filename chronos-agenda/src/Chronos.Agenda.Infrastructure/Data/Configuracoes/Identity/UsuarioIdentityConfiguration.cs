using Chronos.Agenda.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Agenda.Infrastructure.Data.Configuracoes.Identity;

/// <summary>Mapeia o usuário do Identity para a tabela <c>usuarios</c>, sem o prefixo <c>AspNet</c> (ADR 0001), com
/// limites de tamanho defensivos para os campos de texto expostos antes da autenticação.</summary>
public sealed class UsuarioIdentityConfiguration : IEntityTypeConfiguration<UsuarioIdentity>
{
    public void Configure(EntityTypeBuilder<UsuarioIdentity> builder)
    {
        builder.ToTable("usuarios");

        builder.Property(u => u.UserName).HasMaxLength(256);
        builder.Property(u => u.NormalizedUserName).HasMaxLength(256);
        builder.Property(u => u.Email).HasMaxLength(256);
        builder.Property(u => u.NormalizedEmail).HasMaxLength(256);
        builder.Property(u => u.PasswordHash).HasMaxLength(512);
        builder.Property(u => u.SecurityStamp).HasMaxLength(100);
        builder.Property(u => u.ConcurrencyStamp).HasMaxLength(100);
        builder.Property(u => u.PhoneNumber).HasMaxLength(20);
    }
}
