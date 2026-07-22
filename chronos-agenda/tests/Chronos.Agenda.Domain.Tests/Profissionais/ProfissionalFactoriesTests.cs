using Chronos.Agenda.Domain.Profissionais.Entidades;
using Chronos.Agenda.Domain.Profissionais.EventosDominio;
using Chronos.Agenda.Domain.Profissionais.Exceptions;
using Chronos.Agenda.Domain.Profissionais.ObjetosValor;
using Chronos.Agenda.Domain.Tests.Compartilhado;

namespace Chronos.Agenda.Domain.Tests.Profissionais;

public sealed class ProfissionalFactoriesTests
{
    private static readonly DateTime AgoraUtc = new(2026, 7, 21, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Criar_GeraIdentidadeEAtribuiAuditoriaInicial()
    {
        var profissional = Profissional.Criar(Guid.NewGuid(), new NomeProfissional("Marina Costa"), new FakeProvedorDataHora(AgoraUtc));

        Assert.NotEqual(Guid.Empty, profissional.Id);
        Assert.Equal(AgoraUtc, profissional.Auditoria.CriadoEmUtc);
        Assert.Equal(AgoraUtc, profissional.Auditoria.AtualizadoEmUtc);
    }

    [Fact]
    public void Criar_QuandoOrganizacaoInvalida_LancaExcecaoEspecifica()
    {
        var excecao = Assert.Throws<OrganizacaoProfissionalInvalidaException>(
            () => Profissional.Criar(Guid.Empty, new NomeProfissional("Marina Costa"), new FakeProvedorDataHora(AgoraUtc)));

        Assert.Equal($"O profissional deve pertencer a uma organização válida; organização recebida: {Guid.Empty}.", excecao.Message);
    }

    [Fact]
    public void Criar_LancaEventoDeDominio()
    {
        var organizacaoId = Guid.NewGuid();

        var profissional = Profissional.Criar(organizacaoId, new NomeProfissional("Marina Costa"), new FakeProvedorDataHora(AgoraUtc));

        var evento = Assert.IsType<ProfissionalCriado>(
            Assert.Single(profissional.ObterEventosDominio()));
        Assert.Equal(profissional.Id, evento.ProfissionalId);
        Assert.Equal(organizacaoId, evento.OrganizacaoId);
        Assert.Equal(AgoraUtc, evento.OcorridoEmUtc);
    }

    [Fact]
    public void LimparEventosDominio_RemoveEventosPendentes()
    {
        var profissional = Profissional.Criar(Guid.NewGuid(), new NomeProfissional("Marina Costa"), new FakeProvedorDataHora(AgoraUtc));

        profissional.LimparEventosDominio();

        Assert.Empty(profissional.ObterEventosDominio());
    }

}
