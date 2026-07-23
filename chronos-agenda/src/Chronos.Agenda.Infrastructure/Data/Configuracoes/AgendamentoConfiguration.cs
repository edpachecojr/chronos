using Chronos.Agenda.Domain.Agendamentos.Entidades;
using Chronos.Agenda.Domain.Organizacoes.Entidades;
using Chronos.Agenda.Domain.Profissionais.Entidades;
using Chronos.Agenda.Domain.Servicos.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Agenda.Infrastructure.Data.Configuracoes;

/// <summary>Mapeia <see cref="Agendamento"/> para a tabela <c>agendamentos</c>. A restrição pelo tenant
/// (<c>OrganizacaoId</c>) é aplicada explicitamente pelos repositórios (RN01, ADR 0001).</summary>
public sealed class AgendamentoConfiguration : IEntityTypeConfiguration<Agendamento>
{
    public void Configure(EntityTypeBuilder<Agendamento> builder)
    {
        builder.ToTable("agendamentos");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.OrganizacaoId).IsRequired();
        builder.Property(a => a.ProfissionalId).IsRequired();
        builder.Property(a => a.ServicoId).IsRequired();
        builder.Property(a => a.NomeServicoContratado).HasColumnName("nome_servico_contratado").HasMaxLength(120).IsRequired();
        builder.Property(a => a.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.HasIndex(a => new { a.OrganizacaoId, a.ProfissionalId });
        builder.HasIndex(a => new { a.ProfissionalId, a.Status });

        builder.HasOne<Organizacao>()
            .WithMany()
            .HasForeignKey(a => a.OrganizacaoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Profissional>()
            .WithMany()
            .HasForeignKey(a => a.ProfissionalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Servico>()
            .WithMany()
            .HasForeignKey(a => a.ServicoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(a => a.PessoaAtendida, pessoa =>
        {
            pessoa.OwnsOne(p => p.Nome, nome =>
            {
                nome.Property(n => n.Valor).HasColumnName("pessoa_atendida_nome").HasMaxLength(120).IsRequired();
            });
            pessoa.Property(p => p.Tipo).HasColumnName("pessoa_atendida_tipo").HasConversion<string>().HasMaxLength(20).IsRequired();
        });

        builder.OwnsOne(a => a.Periodo, periodo =>
        {
            periodo.Property(p => p.InicioUtc).HasColumnName("inicio_utc").IsRequired();
            periodo.Property(p => p.FimUtc).HasColumnName("fim_utc").IsRequired();
            periodo.Ignore(p => p.Duracao);
            periodo.Ignore(p => p.DuracaoEmMinutos);
            periodo.HasIndex(p => new { p.InicioUtc, p.FimUtc });
        });

        builder.OwnsOne(a => a.PrecoCobrado, preco =>
        {
            preco.Property(p => p.Valor).HasColumnName("preco_cobrado").HasColumnType("numeric(10,2)").IsRequired();
        });

        builder.OwnsOne(a => a.Local, local =>
        {
            local.Property(l => l.Tipo).HasColumnName("local_tipo").HasConversion<string>().HasMaxLength(30).IsRequired();
            local.OwnsOne(l => l.Endereco, endereco =>
            {
                endereco.Property(e => e.Descricao).HasColumnName("local_endereco").HasMaxLength(300).IsRequired(false);
                endereco.Property<byte?>("_discriminator").IsRequired();
            });
        });

        builder.OwnsAuditoria();

        builder.Ignore(a => a.TipoAtendimento);
        builder.Ignore(a => a.DuracaoReservada);
    }
}
