using Chronos.Agenda.Application.Compartilhado;
using Chronos.Agenda.Application.Profissionais.ListarProfissionais;
using Chronos.Agenda.Application.Tests.Compartilhado.Fakes;
using Chronos.Agenda.Application.Tests.Profissionais.Fakes;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Profissionais.Entidades;

namespace Chronos.Agenda.Application.Tests.Profissionais.ListarProfissionais;

public class ListarProfissionaisHandlerTests
{
    private static readonly DateTime CriadoEmUtc = new(2026, 7, 22, 12, 0, 0, DateTimeKind.Utc);

    private readonly Guid _organizacaoId = Guid.NewGuid();
    private readonly FakeProfissionalRepositorio _profissionalRepositorio = new();
    private readonly FakeProvedorDataHora _provedorDataHora = new(CriadoEmUtc);
    private readonly ListarProfissionaisHandler _handler;

    public ListarProfissionaisHandlerTests()
    {
        _handler = new ListarProfissionaisHandler(_profissionalRepositorio, new ContextoUsuario(Guid.NewGuid(), _organizacaoId));
    }

    [Fact]
    public async Task ExecutarAsync_retorna_profissionais_da_organizacao_ordenados_por_nome()
    {
        var joao = Profissional.Criar(_organizacaoId, new Nome("João Lima"), _provedorDataHora);
        var ana = Profissional.Criar(_organizacaoId, new Nome("Ana Souza"), _provedorDataHora);
        await _profissionalRepositorio.AdicionarAsync(joao, CancellationToken.None);
        await _profissionalRepositorio.AdicionarAsync(ana, CancellationToken.None);

        var resultado = await _handler.ExecutarAsync(CancellationToken.None);

        Assert.Equal(2, resultado.Count);
        Assert.Equal([ana.Id, joao.Id], resultado.Select(r => r.ProfissionalId));
    }

    [Fact]
    public async Task ExecutarAsync_nao_retorna_profissionais_de_outra_organizacao()
    {
        var deOutraOrganizacao = Profissional.Criar(Guid.NewGuid(), new Nome("Dr. João Lima"), _provedorDataHora);
        await _profissionalRepositorio.AdicionarAsync(deOutraOrganizacao, CancellationToken.None);

        var resultado = await _handler.ExecutarAsync(CancellationToken.None);

        Assert.Empty(resultado);
    }
}
