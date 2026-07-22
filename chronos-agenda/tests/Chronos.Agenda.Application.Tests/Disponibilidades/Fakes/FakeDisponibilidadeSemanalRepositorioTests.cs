using Chronos.Agenda.Application.Tests.Compartilhado.Fakes;
using Chronos.Agenda.Domain.Disponibilidades.Entidades;
using Chronos.Agenda.Domain.Disponibilidades.ObjetosValor;

namespace Chronos.Agenda.Application.Tests.Disponibilidades.Fakes;

public class FakeDisponibilidadeSemanalRepositorioTests
{
    private readonly FakeDisponibilidadeSemanalRepositorio _repositorio = new();
    private readonly FakeProvedorDataHora _provedorDataHora = new(DateTime.UtcNow);

    [Fact]
    public async Task BuscarPorProfissionalEDia_ignora_disponibilidades_de_outro_dia_ou_organizacao()
    {
        var organizacaoId = Guid.NewGuid();
        var profissionalId = Guid.NewGuid();
        var janela = new JanelaHorario(new TimeOnly(9, 0), new TimeOnly(12, 0));
        var doDiaCorreto = DisponibilidadeSemanal.Criar(organizacaoId, profissionalId, DayOfWeek.Monday, janela, _provedorDataHora);
        var deOutroDia = DisponibilidadeSemanal.Criar(organizacaoId, profissionalId, DayOfWeek.Tuesday, janela, _provedorDataHora);
        var deOutraOrganizacao = DisponibilidadeSemanal.Criar(Guid.NewGuid(), profissionalId, DayOfWeek.Monday, janela, _provedorDataHora);
        await _repositorio.AdicionarAsync(doDiaCorreto, CancellationToken.None);
        await _repositorio.AdicionarAsync(deOutroDia, CancellationToken.None);
        await _repositorio.AdicionarAsync(deOutraOrganizacao, CancellationToken.None);

        var encontradas = await _repositorio.BuscarPorProfissionalEDiaAsync(organizacaoId, profissionalId, DayOfWeek.Monday, CancellationToken.None);

        Assert.Equal([doDiaCorreto], encontradas);
    }

    [Fact]
    public async Task Remover_exclui_a_disponibilidade_das_buscas_seguintes()
    {
        var organizacaoId = Guid.NewGuid();
        var profissionalId = Guid.NewGuid();
        var disponibilidade = DisponibilidadeSemanal.Criar(
            organizacaoId, profissionalId, DayOfWeek.Monday, new JanelaHorario(new TimeOnly(9, 0), new TimeOnly(12, 0)), _provedorDataHora);
        await _repositorio.AdicionarAsync(disponibilidade, CancellationToken.None);

        await _repositorio.RemoverAsync(disponibilidade, CancellationToken.None);

        var encontradas = await _repositorio.BuscarPorProfissionalAsync(organizacaoId, profissionalId, CancellationToken.None);
        Assert.Empty(encontradas);
    }
}
