using Chronos.Agenda.Application.Tests.Compartilhado.Fakes;
using Chronos.Agenda.Domain.Servicos.Entidades;
using Chronos.Agenda.Domain.Servicos.Enums;
using Chronos.Agenda.Domain.Servicos.ObjetosValor;

namespace Chronos.Agenda.Application.Tests.Servicos.Fakes;

public class FakeServicoRepositorioTests
{
    private readonly FakeServicoRepositorio _repositorio = new();
    private readonly FakeProvedorDataHora _provedorDataHora = new(DateTime.UtcNow);

    [Fact]
    public async Task BuscarPorId_com_organizacao_diferente_da_dona_retorna_nulo()
    {
        var servico = CriarServico(Guid.NewGuid(), new PrecoServico(100m));
        await _repositorio.AdicionarAsync(servico, CancellationToken.None);

        var encontrado = await _repositorio.BuscarPorIdAsync(Guid.NewGuid(), servico.Id, CancellationToken.None);

        Assert.Null(encontrado);
    }

    [Fact]
    public async Task Atualizar_substitui_o_servico_persistido()
    {
        var organizacaoId = Guid.NewGuid();
        var servico = CriarServico(organizacaoId, new PrecoServico(100m));
        await _repositorio.AdicionarAsync(servico, CancellationToken.None);

        servico.Atualizar(servico.Nome, servico.Duracao, new PrecoServico(150m), servico.TipoAtendimento, _provedorDataHora);
        await _repositorio.AtualizarAsync(servico, CancellationToken.None);

        var encontrado = await _repositorio.BuscarPorIdAsync(organizacaoId, servico.Id, CancellationToken.None);
        Assert.Equal(150m, encontrado!.Preco.Valor);
    }

    private Servico CriarServico(Guid organizacaoId, PrecoServico preco)
    {
        return Servico.Criar(
            organizacaoId,
            Guid.NewGuid(),
            new NomeServico("Consulta"),
            new DuracaoServico(TimeSpan.FromMinutes(30)),
            preco,
            TipoAtendimento.Online,
            _provedorDataHora);
    }
}
