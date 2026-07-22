using Chronos.Agenda.Domain.Agendamentos.Exceptions;
using Chronos.Agenda.Domain.Agendamentos.ObjetosValor;
using Chronos.Agenda.Domain.Agendamentos.Servicos;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Servicos.Enums;

namespace Chronos.Agenda.Domain.Tests.Agendamentos;

public sealed class VerificadorCompatibilidadeLocalTests
{
    [Fact]
    public void Verificar_QuandoModalidadeCorrespondeAoLocal_NaoLancaExcecao()
    {
        var local = LocalAtendimento.Domiciliar(new EnderecoAtendimento("Rua Exemplo, 1"));

        VerificadorCompatibilidadeLocal.Verificar(TipoAtendimento.Domiciliar, local);
    }

    [Fact]
    public void Verificar_QuandoModalidadeNaoCorrespondeAoLocal_LancaExcecaoEspecifica()
    {
        var local = LocalAtendimento.Online();

        var excecao = Assert.Throws<LocalIncompativelComServicoException>(
            () => VerificadorCompatibilidadeLocal.Verificar(TipoAtendimento.Domiciliar, local));

        Assert.Contains("Domiciliar", excecao.Message);
        Assert.Contains("Online", excecao.Message);
    }
}
