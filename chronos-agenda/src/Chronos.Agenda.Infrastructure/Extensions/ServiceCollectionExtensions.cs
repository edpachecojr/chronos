using Chronos.Agenda.Application.Agendamentos.Contratos;
using Chronos.Agenda.Application.Compartilhado.Contratos;
using Chronos.Agenda.Application.Disponibilidades.Contratos;
using Chronos.Agenda.Application.Organizacoes.Contratos;
using Chronos.Agenda.Application.Profissionais.Contratos;
using Chronos.Agenda.Application.Servicos.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Infrastructure.Agendamentos;
using Chronos.Agenda.Infrastructure.Compartilhado;
using Chronos.Agenda.Infrastructure.Data;
using Chronos.Agenda.Infrastructure.Disponibilidades;
using Chronos.Agenda.Infrastructure.Identity;
using Chronos.Agenda.Infrastructure.Organizacoes;
using Chronos.Agenda.Infrastructure.Profissionais;
using Chronos.Agenda.Infrastructure.Servicos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chronos.Agenda.Infrastructure.Extensions;

/// <summary>Registra os serviços concretos da Infraestrutura no contêiner de injeção de dependências. A camada de
/// Api compõe esses serviços sem conhecer os detalhes de EF Core/PostgreSQL/Identity (ADR 0001).</summary>
public static class ServiceCollectionExtensions
{
    private const string NomeDaStringDeConexao = "ChronosAgenda";

    /// <summary>Registra o <see cref="ChronosAgendaDbContext"/> configurado para PostgreSQL, com convenção
    /// <c>snake_case</c> (ADR 0001). A string de conexão vem de configuração externa (variável de ambiente ou
    /// gerenciador de segredos), nunca de valor fixo no código.</summary>
    /// <example><code>builder.Services.AdicionarPersistencia(builder.Configuration);</code></example>
    public static IServiceCollection AdicionarPersistencia(this IServiceCollection servicos, IConfiguration configuracao)
    {
        var stringDeConexao = configuracao.GetConnectionString(NomeDaStringDeConexao)
            ?? throw new InvalidOperationException(
                $"A string de conexão '{NomeDaStringDeConexao}' não foi configurada. Defina a variável de ambiente " +
                $"'ConnectionStrings__{NomeDaStringDeConexao}' ou a configuração equivalente.");

        servicos.AddDbContext<ChronosAgendaDbContext>(opcoes => opcoes
            .UseNpgsql(stringDeConexao)
            .UseSnakeCaseNamingConvention());

        return servicos;
    }

    /// <summary>Registra o ASP.NET Core Identity com <see cref="UsuarioIdentity"/>/<see cref="PapelIdentity"/>
    /// (chave <see cref="Guid"/>) e tokens de portador (bearer) como mecanismo de sessão. Não mapeia os endpoints
    /// de Identity: isso é responsabilidade da composição final da Api, quando os endpoints existirem.</summary>
    /// <example><code>builder.Services.AdicionarIdentity();</code></example>
    public static IServiceCollection AdicionarIdentity(this IServiceCollection servicos)
    {
        servicos
            .AddIdentityApiEndpoints<UsuarioIdentity>(opcoes =>
            {
                opcoes.User.RequireUniqueEmail = true;
            })
            .AddRoles<PapelIdentity>()
            .AddEntityFrameworkStores<ChronosAgendaDbContext>();

        return servicos;
    }

    /// <summary>Registra os repositórios, a unidade de trabalho e o provedor de data/hora concretos usados pelos
    /// casos de uso da Aplicação.</summary>
    /// <example><code>builder.Services.AdicionarRepositorios();</code></example>
    public static IServiceCollection AdicionarRepositorios(this IServiceCollection servicos)
    {
        servicos.AddScoped<IOrganizacaoRepositorio, OrganizacaoRepositorio>();
        servicos.AddScoped<IProfissionalRepositorio, ProfissionalRepositorio>();
        servicos.AddScoped<IServicoRepositorio, ServicoRepositorio>();
        servicos.AddScoped<IDisponibilidadeSemanalRepositorio, DisponibilidadeSemanalRepositorio>();
        servicos.AddScoped<IAgendamentoRepositorio, AgendamentoRepositorio>();
        servicos.AddScoped<IMembroOrganizacaoRepositorio, MembroOrganizacaoRepositorio>();
        servicos.AddScoped<IUnidadeDeTrabalho, UnidadeDeTrabalho>();
        servicos.AddSingleton<IProvedorDataHora, ProvedorDataHoraUtc>();

        return servicos;
    }

    /// <summary>Registra a implementação concreta do serviço de autenticação sobre o ASP.NET Core Identity.</summary>
    /// <example><code>builder.Services.AdicionarServicoAutenticacao();</code></example>
    public static IServiceCollection AdicionarServicoAutenticacao(this IServiceCollection servicos)
    {
        servicos.AddScoped<IServicoAutenticacao, ServicoAutenticacao>();
        return servicos;
    }
}
