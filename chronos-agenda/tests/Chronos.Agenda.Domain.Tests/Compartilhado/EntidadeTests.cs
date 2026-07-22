using Chronos.Agenda.Domain.Compartilhado.Entidades;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;

namespace Chronos.Agenda.Domain.Tests.Compartilhado;

public sealed class EntidadeTests
{
    private static readonly DateTime CriadoEmUtc = new(2026, 7, 21, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Equals_QuandoIdentidadesSaoIguais_RetornaVerdadeiro()
    {
        var id = Guid.NewGuid();
        var primeira = new EntidadeDeTeste(id);
        var segunda = new EntidadeDeTeste(id);

        Assert.Equal(primeira, segunda);
        Assert.Equal(primeira.GetHashCode(), segunda.GetHashCode());
    }

    [Fact]
    public void Equals_QuandoIdentidadesSaoDiferentes_RetornaFalso()
    {
        var primeira = new EntidadeDeTeste(Guid.NewGuid());
        var segunda = new EntidadeDeTeste(Guid.NewGuid());

        Assert.NotEqual(primeira, segunda);
    }

    private sealed class EntidadeDeTeste(Guid id) : Entidade(id, Auditoria.Criar(new FakeProvedorDataHora(CriadoEmUtc)));
}
