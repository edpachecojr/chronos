using Chronos.Agenda.Domain.Compartilhado.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Agenda.Infrastructure.Data.Configuracoes;

/// <summary>Mapeamento compartilhado por todas as entidades do domínio, que herdam <see cref="Entidade.Auditoria"/>.</summary>
internal static class EntityTypeBuilderExtensions
{
    public static void OwnsAuditoria<TEntidade>(this EntityTypeBuilder<TEntidade> builder)
        where TEntidade : Entidade
    {
        builder.OwnsOne(entidade => entidade.Auditoria, auditoria =>
        {
            auditoria.Property(a => a.CriadoEmUtc).HasColumnName("criado_em_utc").IsRequired();
            auditoria.Property(a => a.AtualizadoEmUtc).HasColumnName("atualizado_em_utc").IsRequired();
        });
    }
}
