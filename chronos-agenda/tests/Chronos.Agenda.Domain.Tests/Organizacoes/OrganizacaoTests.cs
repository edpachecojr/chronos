using Chronos.Agenda.Domain.Organizacoes.Excecoes;
using Chronos.Agenda.Domain.Organizacoes.ObjetosValor;

namespace Chronos.Agenda.Domain.Tests.Organizacoes;

public sealed class OrganizacaoTests
{
    [Fact]
    public void NomeOrganizacao_QuandoVazio_LancaExcecaoEspecifica()
    {
        Assert.Throws<NomeOrganizacaoInvalidoException>(() => new NomeOrganizacao("  "));
    }
}
