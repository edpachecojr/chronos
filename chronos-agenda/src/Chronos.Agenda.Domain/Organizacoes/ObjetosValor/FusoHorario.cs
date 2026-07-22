using Chronos.Agenda.Domain.Organizacoes.Exceptions;

namespace Chronos.Agenda.Domain.Organizacoes.ObjetosValor;

/// <summary>Representa o fuso horário IANA usado para interpretar o expediente de uma organização.</summary>
public sealed record FusoHorario
{
    public FusoHorario(string identificador)
    {
        var identificadorNormalizado = identificador?.Trim();
        if (string.IsNullOrWhiteSpace(identificadorNormalizado))
        {
            throw new FusoHorarioInvalidoException(identificadorNormalizado ?? string.Empty);
        }

        if (!ExisteFusoHorarioIana(identificadorNormalizado))
        {
            throw new FusoHorarioInvalidoException(identificadorNormalizado);
        }

        Identificador = identificadorNormalizado;
    }

    public string Identificador { get; }

    private static bool ExisteFusoHorarioIana(string identificador)
    {
        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(identificador);
            return true;
        }
        catch (Exception excecao) when (excecao is TimeZoneNotFoundException or InvalidTimeZoneException)
        {
            return false;
        }
    }
}
