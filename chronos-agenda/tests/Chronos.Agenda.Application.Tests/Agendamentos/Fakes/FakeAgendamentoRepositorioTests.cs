using Chronos.Agenda.Application.Tests.Compartilhado.Fakes;
using Chronos.Agenda.Domain.Agendamentos.Entidades;
using Chronos.Agenda.Domain.Agendamentos.Enums;
using Chronos.Agenda.Domain.Agendamentos.ObjetosValor;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Servicos.ObjetosValor;

namespace Chronos.Agenda.Application.Tests.Agendamentos.Fakes;

public class FakeAgendamentoRepositorioTests
{
    private readonly FakeAgendamentoRepositorio _repositorio = new();
    private readonly FakeProvedorDataHora _provedorDataHora = new(DateTime.UtcNow);
    private static readonly DateTime InicioBase = new(2026, 7, 27, 9, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task BuscarAtivosSobrepostos_ignora_cancelados_e_outras_organizacoes()
    {
        var organizacaoId = Guid.NewGuid();
        var profissionalId = Guid.NewGuid();
        var periodoNovo = new PeriodoAgendamento(InicioBase, InicioBase.AddMinutes(30));

        var ativoSobreposto = CriarAgendamento(organizacaoId, profissionalId, InicioBase.AddMinutes(-15));
        var cancelado = CriarAgendamento(organizacaoId, profissionalId, InicioBase.AddMinutes(-15));
        cancelado.Cancelar(_provedorDataHora);
        var deOutraOrganizacao = CriarAgendamento(Guid.NewGuid(), profissionalId, InicioBase.AddMinutes(-15));
        await _repositorio.AdicionarAsync(ativoSobreposto, CancellationToken.None);
        await _repositorio.AdicionarAsync(cancelado, CancellationToken.None);
        await _repositorio.AdicionarAsync(deOutraOrganizacao, CancellationToken.None);

        var encontrados = await _repositorio.BuscarAtivosSobrepostosAsync(organizacaoId, profissionalId, periodoNovo, CancellationToken.None);

        Assert.Equal([ativoSobreposto], encontrados);
    }

    [Fact]
    public async Task BuscarPorId_com_organizacao_diferente_da_dona_retorna_nulo()
    {
        var agendamento = CriarAgendamento(Guid.NewGuid(), Guid.NewGuid(), InicioBase);
        await _repositorio.AdicionarAsync(agendamento, CancellationToken.None);

        var encontrado = await _repositorio.BuscarPorIdAsync(Guid.NewGuid(), agendamento.Id, CancellationToken.None);

        Assert.Null(encontrado);
    }

    private Agendamento CriarAgendamento(Guid organizacaoId, Guid profissionalId, DateTime inicioUtc)
    {
        return Agendamento.Criar(
            organizacaoId,
            profissionalId,
            Guid.NewGuid(),
            "Consulta",
            new PessoaAtendida(new Nome("Cliente Exemplo"), TipoPessoaAtendida.Cliente),
            new PeriodoAgendamento(inicioUtc, inicioUtc.AddMinutes(30)),
            new PrecoServico(100m),
            LocalAtendimento.Online(),
            _provedorDataHora);
    }
}
