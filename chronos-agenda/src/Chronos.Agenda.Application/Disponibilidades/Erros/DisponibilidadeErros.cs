using Chronos.Agenda.Domain.Compartilhado.Resultados;

namespace Chronos.Agenda.Application.Disponibilidades.Erros;

/// <summary>Catálogo de erros esperados nos casos de uso de disponibilidade semanal.</summary>
public static class DisponibilidadeErros
{
    /// <summary>A janela informada é inválida (o fim não é posterior ao início).</summary>
    public static Erro JanelaInvalida(string mensagem) => new("Disponibilidade.JanelaInvalida", mensagem);

    /// <summary>A janela informada se sobrepõe a outra disponibilidade já configurada do mesmo profissional no
    /// mesmo dia da semana (Fase B item 8).</summary>
    public static readonly Erro JanelaSobreposta = new(
        "Disponibilidade.JanelaSobreposta",
        "A janela informada se sobrepõe a outra disponibilidade já configurada para este profissional neste dia.");

    /// <summary>A disponibilidade não existe ou pertence a outro profissional/organização (RN01).</summary>
    public static Erro NaoEncontrada(Guid disponibilidadeId) => new(
        "Disponibilidade.NaoEncontrada",
        $"Nenhuma disponibilidade {disponibilidadeId} foi encontrada para este profissional.");
}
