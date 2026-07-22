using Chronos.Agenda.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Agenda.Infrastructure.Data.Configuracoes.Identity;

/// <summary>Mapeia o papel do Identity para a tabela <c>roles</c>, sem o prefixo <c>AspNet</c> (ADR 0001).</summary>
public sealed class PapelIdentityConfiguration : IEntityTypeConfiguration<PapelIdentity>
{
    public void Configure(EntityTypeBuilder<PapelIdentity> builder)
    {
        builder.ToTable("roles");

        builder.Property(p => p.Name).HasMaxLength(100);
        builder.Property(p => p.NormalizedName).HasMaxLength(100);
        builder.Property(p => p.ConcurrencyStamp).HasMaxLength(100);
    }
}
