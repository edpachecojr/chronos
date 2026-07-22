using Chronos.Agenda.Domain.Compartilhado;
using Chronos.Agenda.Domain.Servicos;

namespace Chronos.Agenda.Domain.Tests.Servicos;

public sealed class ServicoTests
{
    [Fact]
    public void Atualizar_SubstituiConfiguracaoComercial()
    {
        var criadoEmUtc = new DateTime(2026, 7, 21, 12, 0, 0, DateTimeKind.Utc);
        var servico = Servico.Criar(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new NomeServico("Corte"),
            new DuracaoServico(TimeSpan.FromMinutes(30)),
            new PrecoServico(50m),
            TipoAtendimento.Presencial,
            criadoEmUtc);

        servico.Atualizar(
            new NomeServico("Corte e barba"),
            new DuracaoServico(TimeSpan.FromMinutes(60)),
            new PrecoServico(90m),
            TipoAtendimento.Domiciliar,
            criadoEmUtc.AddMinutes(1));

        Assert.Equal("Corte e barba", servico.Nome.Valor);
        Assert.Equal(TimeSpan.FromMinutes(60), servico.Duracao.Valor);
        Assert.Equal(90m, servico.Preco.Valor);
        Assert.Equal(TipoAtendimento.Domiciliar, servico.TipoAtendimento);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(10.999)]
    public void PrecoServico_QuandoInvalido_LancaExcecaoDeDominio(decimal valor)
    {
        Assert.Throws<DomainException>(() => new PrecoServico(valor));
    }
}
