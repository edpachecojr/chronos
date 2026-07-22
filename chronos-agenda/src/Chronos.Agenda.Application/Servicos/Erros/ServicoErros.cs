using Chronos.Agenda.Domain.Compartilhado.Resultados;

namespace Chronos.Agenda.Application.Servicos.Erros;

/// <summary>Catálogo de erros esperados nos casos de uso de serviço.</summary>
public static class ServicoErros
{
    /// <summary>O nome informado para o serviço é inválido (comprimento fora do limite).</summary>
    public static Erro NomeInvalido(string mensagem) => new("Servico.NomeInvalido", mensagem);

    /// <summary>A duração informada para o serviço é inválida (fora do intervalo permitido).</summary>
    public static Erro DuracaoInvalida(string mensagem) => new("Servico.DuracaoInvalida", mensagem);

    /// <summary>O preço informado para o serviço é inválido (negativo ou com mais de duas casas decimais).</summary>
    public static Erro PrecoInvalido(string mensagem) => new("Servico.PrecoInvalido", mensagem);

    /// <summary>O serviço não existe ou pertence a outra organização (RN01).</summary>
    public static Erro NaoEncontrado(Guid organizacaoId, Guid servicoId) => new(
        "Servico.NaoEncontrado",
        $"Nenhum serviço {servicoId} foi encontrado na organização {organizacaoId}.");
}
