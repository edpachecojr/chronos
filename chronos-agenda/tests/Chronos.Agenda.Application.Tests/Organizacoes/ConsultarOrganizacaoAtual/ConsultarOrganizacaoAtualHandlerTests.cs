using Chronos.Agenda.Application.Compartilhado;
using Chronos.Agenda.Application.Organizacoes.ConsultarOrganizacaoAtual;
using Chronos.Agenda.Application.Tests.Compartilhado.Fakes;
using Chronos.Agenda.Application.Tests.Organizacoes.Fakes;
using Chronos.Agenda.Domain.Organizacoes.Entidades;
using Chronos.Agenda.Domain.Organizacoes.ObjetosValor;

namespace Chronos.Agenda.Application.Tests.Organizacoes.ConsultarOrganizacaoAtual;

public class ConsultarOrganizacaoAtualHandlerTests
{
    private readonly FakeOrganizacaoRepositorio _organizacaoRepositorio = new();
    private readonly FakeProvedorDataHora _provedorDataHora = new(DateTime.UtcNow);

    [Fact]
    public async Task ExecutarAsync_com_usuario_sem_organizacao_retorna_nulo()
    {
        var contextoUsuario = new ContextoUsuario(Guid.NewGuid(), Guid.Empty);
        var handler = new ConsultarOrganizacaoAtualHandler(_organizacaoRepositorio, contextoUsuario);

        var resultado = await handler.ExecutarAsync(CancellationToken.None);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task ExecutarAsync_com_usuario_vinculado_a_organizacao_retorna_seus_dados()
    {
        var organizacao = Organizacao.Criar(new NomeOrganizacao("Clínica Bem-Estar"), _provedorDataHora);
        await _organizacaoRepositorio.AdicionarAsync(organizacao, CancellationToken.None);
        var contextoUsuario = new ContextoUsuario(Guid.NewGuid(), organizacao.Id);
        var handler = new ConsultarOrganizacaoAtualHandler(_organizacaoRepositorio, contextoUsuario);

        var resultado = await handler.ExecutarAsync(CancellationToken.None);

        Assert.NotNull(resultado);
        Assert.Equal(organizacao.Id, resultado.OrganizacaoId);
        Assert.Equal("Clínica Bem-Estar", resultado.Nome);
    }
}
