using Chronos.Agenda.Application.Tests.Compartilhado.Fakes;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Profissionais.Entidades;

namespace Chronos.Agenda.Application.Tests.Profissionais.Fakes;

public class FakeProfissionalRepositorioTests
{
    private readonly FakeProfissionalRepositorio _repositorio = new();
    private readonly FakeProvedorDataHora _provedorDataHora = new(DateTime.UtcNow);

    [Fact]
    public async Task BuscarPorId_com_a_organizacao_correta_retorna_o_profissional()
    {
        var organizacaoId = Guid.NewGuid();
        var profissional = Profissional.Criar(organizacaoId, new Nome("Ana Souza"), _provedorDataHora);
        await _repositorio.AdicionarAsync(profissional, CancellationToken.None);

        var encontrado = await _repositorio.BuscarPorIdAsync(organizacaoId, profissional.Id, CancellationToken.None);

        Assert.Same(profissional, encontrado);
    }

    [Fact]
    public async Task BuscarPorId_com_organizacao_diferente_da_dona_retorna_nulo()
    {
        var profissional = Profissional.Criar(Guid.NewGuid(), new Nome("Ana Souza"), _provedorDataHora);
        await _repositorio.AdicionarAsync(profissional, CancellationToken.None);

        var encontrado = await _repositorio.BuscarPorIdAsync(Guid.NewGuid(), profissional.Id, CancellationToken.None);

        Assert.Null(encontrado);
    }
}
