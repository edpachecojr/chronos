using Chronos.Agenda.Domain.Organizacoes.Entidades;
using Chronos.Agenda.Domain.Profissionais.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Agenda.Infrastructure.Data.Configuracoes;

/// <summary>Mapeia <see cref="Profissional"/> para a tabela <c>profissionais</c>. A restrição pelo tenant
/// (<c>OrganizacaoId</c>) é aplicada explicitamente pelos repositórios (RN01, ADR 0001), nunca por query filter.</summary>
public sealed class ProfissionalConfiguration : IEntityTypeConfiguration<Profissional>
{
    public void Configure(EntityTypeBuilder<Profissional> builder)
    {
        builder.ToTable("profissionais");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.OrganizacaoId).IsRequired();
        builder.HasIndex(p => p.OrganizacaoId);

        builder.HasOne<Organizacao>()
            .WithMany()
            .HasForeignKey(p => p.OrganizacaoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(p => p.Nome, nome =>
        {
            nome.Property(n => n.Valor).HasColumnName("nome").HasMaxLength(120).IsRequired();
        });

        builder.OwnsAuditoria();
    }
}
