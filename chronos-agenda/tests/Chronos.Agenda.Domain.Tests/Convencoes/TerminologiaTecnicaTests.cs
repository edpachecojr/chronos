using Chronos.Agenda.Domain.Agendamentos.Enums;
using Chronos.Agenda.Domain.Compartilhado.Exceptions;
using Chronos.Agenda.Domain.Servicos.Enums;

namespace Chronos.Agenda.Domain.Tests.Convencoes;

public sealed class TerminologiaTecnicaTests
{
    [Fact]
    public void Deve_expor_enums_no_namespace_tecnico_em_ingles()
    {
        Assert.Equal(
            "Chronos.Agenda.Domain.Agendamentos.Enums",
            typeof(StatusAgendamento).Namespace);
        Assert.Equal(
            "Chronos.Agenda.Domain.Servicos.Enums",
            typeof(TipoAtendimento).Namespace);
    }

    [Fact]
    public void Nao_deve_expor_namespaces_tecnicos_traduzidos()
    {
        var namespaces = typeof(DomainException).Assembly
            .GetTypes()
            .Select(type => type.Namespace)
            .OfType<string>();

        Assert.DoesNotContain(
            namespaces,
            @namespace =>
                @namespace.Contains(".Enumeracoes", StringComparison.Ordinal) ||
                @namespace.Contains(".Excecoes", StringComparison.Ordinal));
    }
}
