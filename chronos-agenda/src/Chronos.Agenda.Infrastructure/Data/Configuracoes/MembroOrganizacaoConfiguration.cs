using Chronos.Agenda.Domain.Organizacoes.Entidades;
using Chronos.Agenda.Infrastructure.Compartilhado;
using Chronos.Agenda.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Agenda.Infrastructure.Data.Configuracoes;

/// <summary>Mapeia <see cref="MembroOrganizacao"/> para a tabela <c>membros_organizacao</c>, o vínculo
/// usuário↔organização decidido pelo ADR 0003. Um usuário pertence a, no máximo, uma organização neste escopo do
/// MVP (índice único em <c>usuario_id</c>).</summary>
public sealed class MembroOrganizacaoConfiguration : IEntityTypeConfiguration<MembroOrganizacao>
{
    public void Configure(EntityTypeBuilder<MembroOrganizacao> builder)
    {
        builder.ToTable("membros_organizacao");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedNever();

        builder.Property(m => m.UsuarioId).IsRequired();
        builder.Property(m => m.OrganizacaoId).IsRequired();
        builder.Property(m => m.Papel).HasColumnName("papel").HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(m => m.CriadoEmUtc).HasColumnName("criado_em_utc").IsRequired();

        builder.HasIndex(m => m.UsuarioId).IsUnique();

        builder.HasOne<UsuarioIdentity>()
            .WithMany()
            .HasForeignKey(m => m.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Organizacao>()
            .WithMany()
            .HasForeignKey(m => m.OrganizacaoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
