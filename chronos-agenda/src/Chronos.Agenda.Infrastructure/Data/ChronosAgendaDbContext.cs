using Chronos.Agenda.Domain.Agendamentos.Entidades;
using Chronos.Agenda.Domain.Disponibilidades.Entidades;
using Chronos.Agenda.Domain.Organizacoes.Entidades;
using Chronos.Agenda.Domain.Profissionais.Entidades;
using Chronos.Agenda.Domain.Servicos.Entidades;
using Chronos.Agenda.Infrastructure.Compartilhado;
using Chronos.Agenda.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Agenda.Infrastructure.Data;

/// <summary>Contexto de persistência do Chronos Agenda. Combina as tabelas do domínio com as do ASP.NET Core
/// Identity (ADR 0001), todas mapeadas explicitamente por classes em <c>Data/Configuracoes</c>.</summary>
public sealed class ChronosAgendaDbContext(DbContextOptions<ChronosAgendaDbContext> opcoes)
    : IdentityDbContext<UsuarioIdentity, PapelIdentity, Guid>(opcoes)
{
    public DbSet<Organizacao> Organizacoes => Set<Organizacao>();
    public DbSet<Profissional> Profissionais => Set<Profissional>();
    public DbSet<Servico> Servicos => Set<Servico>();
    public DbSet<DisponibilidadeSemanal> DisponibilidadesSemanais => Set<DisponibilidadeSemanal>();
    public DbSet<Agendamento> Agendamentos => Set<Agendamento>();
    public DbSet<MembroOrganizacao> MembrosOrganizacao => Set<MembroOrganizacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ChronosAgendaDbContext).Assembly);
    }
}
