using Chronos.Agenda.Domain.Compartilhado.Resultados;

namespace Chronos.Agenda.Application.Profissionais.Erros;

/// <summary>Catálogo de erros esperados nos casos de uso de profissional.</summary>
public static class ProfissionalErros
{
    /// <summary>O nome informado para o profissional é inválido (comprimento fora do limite).</summary>
    public static Erro NomeInvalido(string mensagem) => new("Profissional.NomeInvalido", mensagem);

    /// <summary>O profissional não existe ou pertence a outra organização (RN01).</summary>
    public static Erro NaoEncontrado(Guid organizacaoId, Guid profissionalId) => new(
        "Profissional.NaoEncontrado",
        $"Nenhum profissional {profissionalId} foi encontrado na organização {organizacaoId}.");
}
