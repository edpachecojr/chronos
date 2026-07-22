using Chronos.Agenda.Domain.Agendamentos.Erros;
using Chronos.Agenda.Domain.Agendamentos.ObjetosValor;
using Chronos.Agenda.Domain.Agendamentos.Servicos;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Servicos.Enums;

namespace Chronos.Agenda.Domain.Tests.Agendamentos;

public sealed class VerificadorCompatibilidadeLocalTests
{
    [Fact]
    public void Verificar_QuandoModalidadeCorrespondeAoLocal_RetornaSucesso()
    {
        var local = LocalAtendimento.Domiciliar(new EnderecoAtendimento("Rua Exemplo, 1"));

        var resultado = VerificadorCompatibilidadeLocal.Verificar(TipoAtendimento.Domiciliar, local);

        Assert.True(resultado.Sucesso);
    }

    [Fact]
    public void Verificar_QuandoModalidadeNaoCorrespondeAoLocal_RetornaFalhaComErroEspecifico()
    {
        var local = LocalAtendimento.Online();

        var resultado = VerificadorCompatibilidadeLocal.Verificar(TipoAtendimento.Domiciliar, local);

        Assert.True(resultado.Falhou);
        Assert.Equal(AgendamentoErros.LocalIncompativel(TipoAtendimento.Domiciliar, TipoAtendimento.Online), resultado.Erro);
    }
}
