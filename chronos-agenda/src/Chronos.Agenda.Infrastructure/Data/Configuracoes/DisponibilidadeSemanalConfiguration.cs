using Chronos.Agenda.Domain.Disponibilidades.Entidades;
using Chronos.Agenda.Domain.Organizacoes.Entidades;
using Chronos.Agenda.Domain.Profissionais.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Agenda.Infrastructure.Data.Configuracoes;

/// <summary>Mapeia <see cref="DisponibilidadeSemanal"/> para a tabela <c>disponibilidades_semanais</c>. A
/// restrição pelo tenant (<c>OrganizacaoId</c>) é aplicada explicitamente pelos repositórios (RN01, ADR 0001).</summary>
public sealed class DisponibilidadeSemanalConfiguration : IEntityTypeConfiguration<DisponibilidadeSemanal>
{
    public void Configure(EntityTypeBuilder<DisponibilidadeSemanal> builder)
    {
        builder.ToTable("disponibilidades_semanais");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).ValueGeneratedNever();

        builder.Property(d => d.OrganizacaoId).IsRequired();
        builder.Property(d => d.ProfissionalId).IsRequired();
        builder.Property(d => d.DiaDaSemana).HasColumnName("dia_da_semana").HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.HasIndex(d => new { d.OrganizacaoId, d.ProfissionalId, d.DiaDaSemana });

        builder.HasOne<Organizacao>()
            .WithMany()
            .HasForeignKey(d => d.OrganizacaoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Profissional>()
            .WithMany()
            .HasForeignKey(d => d.ProfissionalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(d => d.Janela, janela =>
        {
            janela.Property(j => j.Inicio).HasColumnName("hora_inicio").IsRequired();
            janela.Property(j => j.Fim).HasColumnName("hora_fim").IsRequired();
        });

        builder.OwnsAuditoria();
    }
}
