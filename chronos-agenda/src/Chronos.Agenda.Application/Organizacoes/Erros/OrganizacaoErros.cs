using Chronos.Agenda.Domain.Compartilhado.Resultados;

namespace Chronos.Agenda.Application.Organizacoes.Erros;

/// <summary>Catálogo de erros esperados nos casos de uso de organização.</summary>
public static class OrganizacaoErros
{
    /// <summary>O nome informado para a organização é inválido (comprimento fora do limite).</summary>
    public static Erro NomeInvalido(string mensagem) => new("Organizacao.NomeInvalido", mensagem);

    /// <summary>A organização corrente não foi encontrada (situação inesperada: o tenant resolvido por
    /// <see cref="Compartilhado.Contratos.IMembroOrganizacaoRepositorio"/> deveria sempre existir).</summary>
    public static Erro NaoEncontrada(Guid organizacaoId) => new(
        "Organizacao.NaoEncontrada",
        $"Nenhuma organização {organizacaoId} foi encontrada.");

    /// <summary>O endereço do prestador informado para o perfil operacional é inválido (comprimento fora do limite).</summary>
    public static Erro EnderecoInvalido(string mensagem) => new("Organizacao.EnderecoInvalido", mensagem);

    /// <summary>O fuso horário informado para o perfil operacional não é um identificador IANA reconhecido.</summary>
    public static Erro FusoHorarioInvalido(string mensagem) => new("Organizacao.FusoHorarioInvalido", mensagem);
}
