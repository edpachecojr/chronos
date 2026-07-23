using Chronos.Agenda.Application.Agendamentos.CancelarAgendamento;
using Chronos.Agenda.Application.Agendamentos.ConfirmarAgendamento;
using Chronos.Agenda.Application.Agendamentos.ConsultarAgendaDiaria;
using Chronos.Agenda.Application.Agendamentos.ConsultarAgendaSemanal;
using Chronos.Agenda.Application.Agendamentos.CriarAgendamento;
using Chronos.Agenda.Application.Agendamentos.ReagendarAgendamento;
using Chronos.Agenda.Application.Disponibilidades.AlterarDisponibilidade;
using Chronos.Agenda.Application.Disponibilidades.CriarDisponibilidade;
using Chronos.Agenda.Application.Disponibilidades.RemoverDisponibilidade;
using Chronos.Agenda.Application.Organizacoes.ConsultarOrganizacaoAtual;
using Chronos.Agenda.Application.Organizacoes.CriarOrganizacao;
using Chronos.Agenda.Application.Servicos.AtualizarServico;
using Chronos.Agenda.Application.Servicos.CriarServico;
using Microsoft.Extensions.DependencyInjection;

namespace Chronos.Agenda.Application.Extensions;

/// <summary>Registra os handlers de casos de uso da Aplicação no contêiner de injeção de dependências. A camada de
/// Api compõe esses handlers sem precisar conhecer cada caso de uso individualmente.</summary>
public static class ServiceCollectionExtensions
{
    /// <example><code>builder.Services.AdicionarCasosDeUso();</code></example>
    public static IServiceCollection AdicionarCasosDeUso(this IServiceCollection servicos)
    {
        servicos.AddScoped<CriarOrganizacaoHandler>();
        servicos.AddScoped<ConsultarOrganizacaoAtualHandler>();
        servicos.AddScoped<CriarServicoHandler>();
        servicos.AddScoped<AtualizarServicoHandler>();
        servicos.AddScoped<CriarDisponibilidadeHandler>();
        servicos.AddScoped<AlterarDisponibilidadeHandler>();
        servicos.AddScoped<RemoverDisponibilidadeHandler>();
        servicos.AddScoped<CriarAgendamentoHandler>();
        servicos.AddScoped<ReagendarAgendamentoHandler>();
        servicos.AddScoped<ConfirmarAgendamentoHandler>();
        servicos.AddScoped<CancelarAgendamentoHandler>();
        servicos.AddScoped<ConsultarAgendaDiariaHandler>();
        servicos.AddScoped<ConsultarAgendaSemanalHandler>();

        return servicos;
    }
}
