using Chronos.Agenda.Domain.Compartilhado.Resultados;

namespace Chronos.Agenda.Application.Compartilhado.Erros;

/// <summary>Catálogo de erros esperados nas operações de autenticação.</summary>
public static class AutenticacaoErros
{
    /// <summary>O e-mail informado já está cadastrado para outro usuário.</summary>
    public static Erro EmailJaCadastrado(string email) => new(
        "Autenticacao.EmailJaCadastrado",
        $"O e-mail '{email}' já está cadastrado.");

    /// <summary>O cadastro falhou por um motivo diferente de e-mail duplicado (ex.: senha fora da política exigida).</summary>
    public static Erro CriacaoInvalida(IEnumerable<string> motivos) => new(
        "Autenticacao.CriacaoInvalida",
        $"Não foi possível criar o usuário: {string.Join("; ", motivos)}.");

    /// <summary>E-mail ou senha não correspondem a um usuário válido. A mensagem não distingue qual dos dois
    /// falhou, para não expor se um e-mail está ou não cadastrado.</summary>
    public static readonly Erro CredenciaisInvalidas = new(
        "Autenticacao.CredenciaisInvalidas",
        "E-mail ou senha inválidos.");
}
