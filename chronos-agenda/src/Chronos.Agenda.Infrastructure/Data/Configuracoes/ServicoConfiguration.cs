using Chronos.Agenda.Domain.Organizacoes.Entidades;
using Chronos.Agenda.Domain.Profissionais.Entidades;
using Chronos.Agenda.Domain.Servicos.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Agenda.Infrastructure.Data.Configuracoes;

/// <summary>Mapeia <see cref="Servico"/> para a tabela <c>servicos</c>. A restrição pelo tenant
/// (<c>OrganizacaoId</c>) é aplicada explicitamente pelos repositórios (RN01, ADR 0001), nunca por query filter.</summary>
public sealed class ServicoConfiguration : IEntityTypeConfiguration<Servico>
{
    public void Configure(EntityTypeBuilder<Servico> builder)
    {
        builder.ToTable("servicos");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.OrganizacaoId).IsRequired();
        builder.Property(s => s.ProfissionalId).IsRequired();
        builder.HasIndex(s => s.OrganizacaoId);
        builder.HasIndex(s => s.ProfissionalId);

        builder.HasOne<Organizacao>()
            .WithMany()
            .HasForeignKey(s => s.OrganizacaoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Profissional>()
            .WithMany()
            .HasForeignKey(s => s.ProfissionalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(s => s.Nome, nome =>
        {
            nome.Property(n => n.Valor).HasColumnName("nome").HasMaxLength(120).IsRequired();
        });

        builder.OwnsOne(s => s.Duracao, duracao =>
        {
            duracao.Property(d => d.Valor).HasColumnName("duracao").IsRequired();
        });

        builder.OwnsOne(s => s.Preco, preco =>
        {
            preco.Property(p => p.Valor).HasColumnName("preco").HasColumnType("numeric(10,2)").IsRequired();
        });

        builder.Property(s => s.TipoAtendimento).HasColumnName("tipo_atendimento").HasConversion<string>().HasMaxLength(30).IsRequired();

        builder.OwnsAuditoria();
    }
}
