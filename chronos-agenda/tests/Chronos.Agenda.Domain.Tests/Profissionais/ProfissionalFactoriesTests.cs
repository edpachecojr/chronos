using Chronos.Agenda.Domain.Compartilhado;
using Chronos.Agenda.Domain.Profissionais;

namespace Chronos.Agenda.Domain.Tests.Profissionais;

public sealed class ProfissionalFactoriesTests
{
    private static readonly DateTime AgoraUtc = new(2026, 7, 21, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Criar_GeraIdentidadeEAtribuiAuditoriaInicial()
    {
        var profissional = Profissional.Criar(Guid.NewGuid(), new NomeProfissional("Marina Costa"), AgoraUtc);

        Assert.NotEqual(Guid.Empty, profissional.Id);
        Assert.Equal(AgoraUtc, profissional.CriadoEmUtc);
        Assert.Equal(AgoraUtc, profissional.AtualizadoEmUtc);
    }

    [Fact]
    public void Criar_QuandoOrganizacaoInvalida_LancaExcecaoDeDominio()
    {
        var excecao = Assert.Throws<DomainException>(
            () => Profissional.Criar(Guid.Empty, new NomeProfissional("Marina Costa"), AgoraUtc));

        Assert.Equal("O profissional deve pertencer a uma organização válida.", excecao.Message);
    }

    [Fact]
    public void Reidratar_PreservaEstadoPersistidoSemExecutarRegrasDeCriacao()
    {
        var id = Guid.NewGuid();
        var atualizadoEmUtc = AgoraUtc.AddMinutes(5);

        var profissional = Profissional.Reidratar(
            id,
            Guid.Empty,
            new NomeProfissional("Marina Costa"),
            AgoraUtc,
            atualizadoEmUtc);

        Assert.Equal(id, profissional.Id);
        Assert.Equal(Guid.Empty, profissional.OrganizacaoId);
        Assert.Equal(atualizadoEmUtc, profissional.AtualizadoEmUtc);
    }
}
