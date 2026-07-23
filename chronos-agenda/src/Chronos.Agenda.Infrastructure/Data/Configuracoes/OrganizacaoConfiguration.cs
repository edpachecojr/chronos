using Chronos.Agenda.Domain.Organizacoes.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Agenda.Infrastructure.Data.Configuracoes;

/// <summary>Mapeia <see cref="Organizacao"/> para a tabela <c>organizacoes</c>, o limite de propriedade e acesso
/// multi-tenant do Chronos (ADR 0001).</summary>
public sealed class OrganizacaoConfiguration : IEntityTypeConfiguration<Organizacao>
{
    public void Configure(EntityTypeBuilder<Organizacao> builder)
    {
        builder.ToTable("organizacoes");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedNever();

        builder.OwnsOne(o => o.Nome, nome =>
        {
            nome.Property(n => n.Valor).HasColumnName("nome").HasMaxLength(120).IsRequired();
        });

        builder.OwnsOne(o => o.EnderecoPrestador, endereco =>
        {
            endereco.Property(e => e.Descricao).HasColumnName("endereco_prestador").HasMaxLength(300).IsRequired(false);
            endereco.Property<byte?>("_discriminator").IsRequired();
        });

        builder.OwnsOne(o => o.FusoHorario, fuso =>
        {
            fuso.Property(f => f.Identificador).HasColumnName("fuso_horario").HasMaxLength(50).IsRequired(false);
            fuso.Property<byte?>("_discriminator").IsRequired();
        });

        builder.OwnsAuditoria();
    }
}
