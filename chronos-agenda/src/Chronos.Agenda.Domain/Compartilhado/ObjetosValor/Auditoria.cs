using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Exceptions;

namespace Chronos.Agenda.Domain.Compartilhado.ObjetosValor;

/// <summary>Registra os instantes de criação e da última atualização de um objeto de domínio.</summary>
public sealed record Auditoria
{
    private Auditoria(DateTime criadoEmUtc)
    {
        CriadoEmUtc = criadoEmUtc;
        AtualizadoEmUtc = criadoEmUtc;
    }

    public DateTime CriadoEmUtc { get; }
    public DateTime AtualizadoEmUtc { get; private set; }

    /// <summary>Cria o registro de auditoria de um novo objeto.</summary>
    /// <example><code>var auditoria = Auditoria.Criar(provedorDataHora);</code></example>
    public static Auditoria Criar(IProvedorDataHora provedorDataHora)
    {
        return new Auditoria(provedorDataHora.UtcNow);
    }

    /// <summary>Registra a última atualização do objeto.</summary>
    /// <example><code>auditoria.Atualizar(provedorDataHora);</code></example>
    public void Atualizar(IProvedorDataHora provedorDataHora)
    {
        var atualizadoEmUtc = provedorDataHora.UtcNow;
        if (atualizadoEmUtc < CriadoEmUtc)
        {
            throw new AtualizacaoAnteriorCriacaoException(CriadoEmUtc, atualizadoEmUtc);
        }

        AtualizadoEmUtc = atualizadoEmUtc;
    }
}
