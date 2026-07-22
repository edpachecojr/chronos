using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Organizacoes.Entidades;
using Chronos.Agenda.Domain.Organizacoes.Exceptions;
using Chronos.Agenda.Domain.Organizacoes.ObjetosValor;
using Chronos.Agenda.Domain.Tests.Compartilhado;

namespace Chronos.Agenda.Domain.Tests.Organizacoes;

public sealed class OrganizacaoTests
{
    private static readonly DateTime AgoraUtc = new(2026, 7, 21, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void NomeOrganizacao_QuandoVazio_LancaExcecaoEspecifica()
    {
        Assert.Throws<NomeOrganizacaoInvalidoException>(() => new NomeOrganizacao("  "));
    }

    [Fact]
    public void ConfigurarPerfilOperacional_QuandoChamado_AtribuiEnderecoEFusoEAtualizaAuditoria()
    {
        var organizacao = CriarOrganizacao();
        var endereco = new EnderecoAtendimento("Av. Central, 20");
        var fusoHorario = new FusoHorario("America/Sao_Paulo");
        var provedorDataHora = new FakeProvedorDataHora(AgoraUtc.AddMinutes(1));

        organizacao.ConfigurarPerfilOperacional(endereco, fusoHorario, provedorDataHora);

        Assert.Equal(endereco, organizacao.EnderecoPrestador);
        Assert.Equal(fusoHorario, organizacao.FusoHorario);
        Assert.Equal(provedorDataHora.UtcNow, organizacao.Auditoria.AtualizadoEmUtc);
    }

    [Fact]
    public void ConfigurarPerfilOperacional_SemEnderecoFisico_PermiteOrganizacaoSomenteOnline()
    {
        var organizacao = CriarOrganizacao();
        var fusoHorario = new FusoHorario("America/Sao_Paulo");

        organizacao.ConfigurarPerfilOperacional(null, fusoHorario, new FakeProvedorDataHora(AgoraUtc.AddMinutes(1)));

        Assert.Null(organizacao.EnderecoPrestador);
        Assert.Equal(fusoHorario, organizacao.FusoHorario);
    }

    private static Organizacao CriarOrganizacao()
    {
        return Organizacao.Criar(new NomeOrganizacao("Clínica Exemplo"), new FakeProvedorDataHora(AgoraUtc));
    }
}
