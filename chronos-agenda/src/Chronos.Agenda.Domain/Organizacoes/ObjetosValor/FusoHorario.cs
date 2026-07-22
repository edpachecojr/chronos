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

    /// <summary>Converte um instante UTC para o horário de parede e o offset deste fuso naquele instante (ADR
    /// 0005). Nunca é ambíguo nem inexistente: essas situações só ocorrem na conversão local → UTC, responsabilidade
    /// da aplicação ao interpretar um offset já explícito na entrada (ver ADR 0005).</summary>
    /// <example><code>var inicioLocal = fusoHorario.ConverterParaLocal(periodo.InicioUtc);</code></example>
    public DateTimeOffset ConverterParaLocal(DateTime instanteUtc)
    {
        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(Identificador);
        var instante = DateTime.SpecifyKind(instanteUtc, DateTimeKind.Utc);
        var horarioLocal = TimeZoneInfo.ConvertTimeFromUtc(instante, timeZoneInfo);
        return new DateTimeOffset(horarioLocal, timeZoneInfo.GetUtcOffset(instante));
    }

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
