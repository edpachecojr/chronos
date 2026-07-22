using Chronos.Agenda.Domain.Agendamentos.Erros;
using Chronos.Agenda.Domain.Agendamentos.ObjetosValor;
using Chronos.Agenda.Domain.Compartilhado.Resultados;
using Chronos.Agenda.Domain.Servicos.Enums;

namespace Chronos.Agenda.Domain.Agendamentos.Servicos;

/// <summary>Verifica se o local informado é compatível com a modalidade contratada do serviço (RN06).</summary>
public static class VerificadorCompatibilidadeLocal
{
    /// <example><code>var resultado = VerificadorCompatibilidadeLocal.Verificar(servico.TipoAtendimento, local);</code></example>
    public static Resultado Verificar(TipoAtendimento tipoAtendimentoServico, LocalAtendimento local)
    {
        ArgumentNullException.ThrowIfNull(local);
        return tipoAtendimentoServico == local.Tipo
            ? Resultado.Ok()
            : Resultado.Falha(AgendamentoErros.LocalIncompativel(tipoAtendimentoServico, local.Tipo));
    }
}
