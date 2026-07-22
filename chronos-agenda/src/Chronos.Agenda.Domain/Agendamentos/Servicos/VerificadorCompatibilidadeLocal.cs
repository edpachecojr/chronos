using Chronos.Agenda.Domain.Agendamentos.Exceptions;
using Chronos.Agenda.Domain.Agendamentos.ObjetosValor;
using Chronos.Agenda.Domain.Servicos.Enums;

namespace Chronos.Agenda.Domain.Agendamentos.Servicos;

/// <summary>Garante que o local informado é compatível com a modalidade contratada do serviço (RN06).</summary>
public static class VerificadorCompatibilidadeLocal
{
    /// <example><code>VerificadorCompatibilidadeLocal.Verificar(servico.TipoAtendimento, local);</code></example>
    public static void Verificar(TipoAtendimento tipoAtendimentoServico, LocalAtendimento local)
    {
        ArgumentNullException.ThrowIfNull(local);
        if (tipoAtendimentoServico != local.Tipo)
        {
            throw new LocalIncompativelComServicoException(tipoAtendimentoServico, local.Tipo);
        }
    }
}
