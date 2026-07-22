using Chronos.Agenda.Application.Agendamentos;
using Chronos.Agenda.Application.Agendamentos.Contratos;
using Chronos.Agenda.Application.Disponibilidades.Contratos;
using Chronos.Agenda.Application.Organizacoes.Contratos;
using Chronos.Agenda.Application.Profissionais.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Resultados;
using Chronos.Agenda.Domain.Organizacoes.ObjetosValor;

namespace Chronos.Agenda.Application.Agendamentos.ConsultarAgendaSemanal;

/// <summary>
/// Projeta a agenda de um profissional nos sete dias locais a partir de
/// <see cref="ConsultarAgendaSemanalConsulta.InicioDaSemana"/> (UC07), reaproveitando a mesma resolução de fuso
/// horário e a mesma projeção diária de <see cref="ConsultarAgendaDiaria.ConsultarAgendaDiariaHandler"/> via
/// <see cref="ProjetorDeAgenda"/>. Leitura pura, sem exceções de domínio esperadas.
/// </summary>
/// <example><code>
/// var resultado = await handler.ExecutarAsync(
///     new ConsultarAgendaSemanalConsulta(profissionalId, new DateOnly(2026, 7, 27)), cancellationToken);
/// </code></example>
public sealed class ConsultarAgendaSemanalHandler(
    IProfissionalRepositorio profissionalRepositorio,
    IOrganizacaoRepositorio organizacaoRepositorio,
    IDisponibilidadeSemanalRepositorio disponibilidadeRepositorio,
    IAgendamentoRepositorio agendamentoRepositorio,
    IContextoUsuario contextoUsuario)
{
    private const int DiasNaSemana = 7;

    private readonly ProjetorDeAgenda _projetor = new(profissionalRepositorio, organizacaoRepositorio, disponibilidadeRepositorio, agendamentoRepositorio);

    public async Task<Resultado<AgendaSemanalResultado>> ExecutarAsync(ConsultarAgendaSemanalConsulta consulta, CancellationToken cancellationToken)
    {
        var organizacaoId = contextoUsuario.ObterOrganizacaoId();

        var fusoHorarioResultado = await _projetor.ResolverFusoHorarioAsync(organizacaoId, consulta.ProfissionalId, cancellationToken);
        if (fusoHorarioResultado.Falhou)
        {
            return Resultado<AgendaSemanalResultado>.Falha(fusoHorarioResultado.Erro!);
        }

        var dias = await ProjetarDiasAsync(organizacaoId, consulta, fusoHorarioResultado.Valor, cancellationToken);
        return Resultado<AgendaSemanalResultado>.Ok(new AgendaSemanalResultado(dias));
    }

    private async Task<List<AgendaDiariaResultado>> ProjetarDiasAsync(
        Guid organizacaoId, ConsultarAgendaSemanalConsulta consulta, FusoHorario fusoHorario, CancellationToken cancellationToken)
    {
        var dias = new List<AgendaDiariaResultado>(DiasNaSemana);
        for (var deslocamento = 0; deslocamento < DiasNaSemana; deslocamento++)
        {
            var data = consulta.InicioDaSemana.AddDays(deslocamento);
            dias.Add(await _projetor.ProjetarDiaAsync(organizacaoId, consulta.ProfissionalId, data, fusoHorario, cancellationToken));
        }

        return dias;
    }
}
