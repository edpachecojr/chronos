using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Tests.Compartilhado;
using Chronos.Agenda.Domain.Usuarios.Entidades;
using Chronos.Agenda.Domain.Usuarios.Enums;
using Chronos.Agenda.Domain.Usuarios.EventosDominio;
using Chronos.Agenda.Domain.Usuarios.Exceptions;

namespace Chronos.Agenda.Domain.Tests.Usuarios;

public sealed class UsuarioFactoriesTests
{
    private static readonly DateTime AgoraUtc = new(2026, 7, 22, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Criar_GeraIdentidadeEAtribuiAuditoriaInicial()
    {
        var usuario = Usuario.Criar(Guid.NewGuid(), new Nome("Marina Costa"), PapelUsuario.Proprietario, new FakeProvedorDataHora(AgoraUtc));

        Assert.NotEqual(Guid.Empty, usuario.Id);
        Assert.Equal(AgoraUtc, usuario.Auditoria.CriadoEmUtc);
        Assert.Equal(AgoraUtc, usuario.Auditoria.AtualizadoEmUtc);
    }

    [Fact]
    public void Criar_AtribuiOrganizacaoENomeEPapelInformados()
    {
        var organizacaoId = Guid.NewGuid();

        var usuario = Usuario.Criar(organizacaoId, new Nome("Marina Costa"), PapelUsuario.Membro, new FakeProvedorDataHora(AgoraUtc));

        Assert.Equal(organizacaoId, usuario.OrganizacaoId);
        Assert.Equal("Marina Costa", usuario.Nome.Valor);
        Assert.Equal(PapelUsuario.Membro, usuario.Papel);
    }

    [Fact]
    public void Criar_QuandoOrganizacaoInvalida_LancaExcecaoEspecifica()
    {
        var excecao = Assert.Throws<OrganizacaoUsuarioInvalidaException>(
            () => Usuario.Criar(Guid.Empty, new Nome("Marina Costa"), PapelUsuario.Proprietario, new FakeProvedorDataHora(AgoraUtc)));

        Assert.Equal($"O usuário deve pertencer a uma organização válida; organização recebida: {Guid.Empty}.", excecao.Message);
    }

    [Fact]
    public void Criar_LancaEventoDeDominio()
    {
        var organizacaoId = Guid.NewGuid();

        var usuario = Usuario.Criar(organizacaoId, new Nome("Marina Costa"), PapelUsuario.Proprietario, new FakeProvedorDataHora(AgoraUtc));

        var evento = Assert.IsType<UsuarioCriado>(
            Assert.Single(usuario.ObterEventosDominio()));
        Assert.Equal(usuario.Id, evento.UsuarioId);
        Assert.Equal(organizacaoId, evento.OrganizacaoId);
        Assert.Equal(PapelUsuario.Proprietario, evento.Papel);
        Assert.Equal(AgoraUtc, evento.OcorridoEmUtc);
    }

    [Fact]
    public void LimparEventosDominio_RemoveEventosPendentes()
    {
        var usuario = Usuario.Criar(Guid.NewGuid(), new Nome("Marina Costa"), PapelUsuario.Proprietario, new FakeProvedorDataHora(AgoraUtc));

        usuario.LimparEventosDominio();

        Assert.Empty(usuario.ObterEventosDominio());
    }

    [Fact]
    public void Renomear_AtualizaNomeEDataDeAtualizacao()
    {
        var usuario = Usuario.Criar(Guid.NewGuid(), new Nome("Marina Costa"), PapelUsuario.Proprietario, new FakeProvedorDataHora(AgoraUtc));
        var provedorDataHora = new FakeProvedorDataHora(AgoraUtc.AddMinutes(1));

        usuario.Renomear(new Nome("Marina Costa Silva"), provedorDataHora);

        Assert.Equal("Marina Costa Silva", usuario.Nome.Valor);
        Assert.Equal(provedorDataHora.UtcNow, usuario.Auditoria.AtualizadoEmUtc);
    }
}
