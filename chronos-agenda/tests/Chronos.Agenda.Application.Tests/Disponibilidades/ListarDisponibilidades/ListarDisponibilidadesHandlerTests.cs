using Chronos.Agenda.Application.Compartilhado;
using Chronos.Agenda.Application.Disponibilidades.ListarDisponibilidades;
using Chronos.Agenda.Application.Tests.Compartilhado.Fakes;
using Chronos.Agenda.Application.Tests.Disponibilidades.Fakes;
using Chronos.Agenda.Domain.Disponibilidades.Entidades;
using Chronos.Agenda.Domain.Disponibilidades.ObjetosValor;

namespace Chronos.Agenda.Application.Tests.Disponibilidades.ListarDisponibilidades;

public class ListarDisponibilidadesHandlerTests
{
    private static readonly DateTime CriadoEmUtc = new(2026, 7, 22, 12, 0, 0, DateTimeKind.Utc);

    private readonly Guid _organizacaoId = Guid.NewGuid();
    private readonly Guid _profissionalId = Guid.NewGuid();
    private readonly FakeDisponibilidadeSemanalRepositorio _disponibilidadeRepositorio = new();
    private readonly FakeProvedorDataHora _provedorDataHora = new(CriadoEmUtc);
    private readonly ListarDisponibilidadesHandler _handler;

    public ListarDisponibilidadesHandlerTests()
    {
        _handler = new ListarDisponibilidadesHandler(_disponibilidadeRepositorio, new ContextoUsuario(Guid.NewGuid(), _organizacaoId));
    }

    [Fact]
    public async Task ExecutarAsync_retorna_disponibilidades_ordenadas_por_dia_e_horario()
    {
        var quarta = CriarDisponibilidade(DayOfWeek.Wednesday, new TimeOnly(9, 0), new TimeOnly(12, 0));
        var segundaTarde = CriarDisponibilidade(DayOfWeek.Monday, new TimeOnly(14, 0), new TimeOnly(18, 0));
        var segundaManha = CriarDisponibilidade(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(12, 0));
        await _disponibilidadeRepositorio.AdicionarAsync(quarta, CancellationToken.None);
        await _disponibilidadeRepositorio.AdicionarAsync(segundaTarde, CancellationToken.None);
        await _disponibilidadeRepositorio.AdicionarAsync(segundaManha, CancellationToken.None);

        var resultado = await _handler.ExecutarAsync(new ListarDisponibilidadesConsulta(_profissionalId), CancellationToken.None);

        Assert.Equal(3, resultado.Count);
        Assert.Equal(
            [segundaManha.Id, segundaTarde.Id, quarta.Id],
            resultado.Select(r => r.DisponibilidadeId));
    }

    [Fact]
    public async Task ExecutarAsync_com_profissional_sem_disponibilidades_retorna_vazio()
    {
        var resultado = await _handler.ExecutarAsync(new ListarDisponibilidadesConsulta(_profissionalId), CancellationToken.None);

        Assert.Empty(resultado);
    }

    private DisponibilidadeSemanal CriarDisponibilidade(DayOfWeek diaDaSemana, TimeOnly inicio, TimeOnly fim)
    {
        return DisponibilidadeSemanal.Criar(_organizacaoId, _profissionalId, diaDaSemana, new JanelaHorario(inicio, fim), _provedorDataHora);
    }
}
