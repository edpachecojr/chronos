using Chronos.Agenda.Domain.Compartilhado.Exceptions;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;

namespace Chronos.Agenda.Domain.Tests.Compartilhado;

public sealed class AuditoriaTests
{
    private static readonly DateTime CriadoEmUtc = new(2026, 7, 21, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Criar_InicializaOsDoisInstantesComADataDeCriacao()
    {
        var auditoria = Auditoria.Criar(new FakeProvedorDataHora(CriadoEmUtc));

        Assert.Equal(CriadoEmUtc, auditoria.CriadoEmUtc);
        Assert.Equal(CriadoEmUtc, auditoria.AtualizadoEmUtc);
    }

    [Fact]
    public void Atualizar_ComInstantePosterior_RegistraUltimaAtualizacao()
    {
        var provedorDataHora = new FakeProvedorDataHora(CriadoEmUtc);
        var auditoria = Auditoria.Criar(provedorDataHora);
        var atualizadoEmUtc = CriadoEmUtc.AddMinutes(1);

        provedorDataHora.UtcNow = atualizadoEmUtc;
        auditoria.Atualizar(provedorDataHora);

        Assert.Equal(atualizadoEmUtc, auditoria.AtualizadoEmUtc);
    }

    [Fact]
    public void Atualizar_ComInstanteAnteriorACriacao_LancaExcecaoEspecifica()
    {
        var provedorDataHora = new FakeProvedorDataHora(CriadoEmUtc);
        var auditoria = Auditoria.Criar(provedorDataHora);
        provedorDataHora.UtcNow = CriadoEmUtc.AddMinutes(-1);

        Assert.Throws<AtualizacaoAnteriorCriacaoException>(() => auditoria.Atualizar(provedorDataHora));
    }
}
