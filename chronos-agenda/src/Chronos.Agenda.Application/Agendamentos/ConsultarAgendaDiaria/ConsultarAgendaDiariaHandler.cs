using Chronos.Agenda.Application.Agendamentos;
using Chronos.Agenda.Application.Agendamentos.Contratos;
using Chronos.Agenda.Application.Disponibilidades.Contratos;
using Chronos.Agenda.Application.Organizacoes.Contratos;
using Chronos.Agenda.Application.Profissionais.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Resultados;

namespace Chronos.Agenda.Application.Agendamentos.ConsultarAgendaDiaria;

/// <summary>
/// Projeta a agenda de um profissional em um único dia local (UC07): janelas de disponibilidade e períodos já
/// ocupados por agendamentos ativos. Leitura pura, sem exceções de domínio esperadas.
/// </summary>
/// <example><code>
/// var resultado = await handler.ExecutarAsync(
///     new ConsultarAgendaDiariaConsulta(profissionalId, new DateOnly(2026, 7, 27)), cancellationToken);
/// </code></example>
public sealed class ConsultarAgendaDiariaHandler(
    IProfissionalRepositorio profissionalRepositorio,
    IOrganizacaoRepositorio organizacaoRepositorio,
    IDisponibilidadeSemanalRepositorio disponibilidadeRepositorio,
    IAgendamentoRepositorio agendamentoRepositorio,
    IContextoUsuario contextoUsuario)
{
    private readonly ProjetorDeAgenda _projetor = new(profissionalRepositorio, organizacaoRepositorio, disponibilidadeRepositorio, agendamentoRepositorio);

    public async Task<Resultado<AgendaDiariaResultado>> ExecutarAsync(ConsultarAgendaDiariaConsulta consulta, CancellationToken cancellationToken)
    {
        var organizacaoId = contextoUsuario.ObterOrganizacaoId();

        var fusoHorarioResultado = await _projetor.ResolverFusoHorarioAsync(organizacaoId, consulta.ProfissionalId, cancellationToken);
        if (fusoHorarioResultado.Falhou)
        {
            return Resultado<AgendaDiariaResultado>.Falha(fusoHorarioResultado.Erro!);
        }

        var agenda = await _projetor.ProjetarDiaAsync(organizacaoId, consulta.ProfissionalId, consulta.Data, fusoHorarioResultado.Valor, cancellationToken);
        return Resultado<AgendaDiariaResultado>.Ok(agenda);
    }
}
