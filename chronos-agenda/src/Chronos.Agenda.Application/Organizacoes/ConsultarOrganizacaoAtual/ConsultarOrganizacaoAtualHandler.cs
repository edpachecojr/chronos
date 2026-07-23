using Chronos.Agenda.Application.Organizacoes.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Contratos;

namespace Chronos.Agenda.Application.Organizacoes.ConsultarOrganizacaoAtual;

/// <summary>
/// Resolve a organização vinculada ao usuário autenticado, para que o
/// frontend saiba se deve encaminhá-lo ao onboarding (UC01) ou ao produto.
/// Não é uma falha esperada o usuário ainda não pertencer a uma organização
/// — por isso o resultado é nulo nesse caso, em vez de modelado como erro do
/// Result Pattern.
/// </summary>
/// <example><code>var resultado = await handler.ExecutarAsync(cancellationToken);</code></example>
public sealed class ConsultarOrganizacaoAtualHandler(
    IOrganizacaoRepositorio organizacaoRepositorio,
    IContextoUsuario contextoUsuario)
{
    public async Task<ConsultarOrganizacaoAtualResultado?> ExecutarAsync(CancellationToken cancellationToken)
    {
        var organizacaoId = contextoUsuario.ObterOrganizacaoId();
        if (organizacaoId == Guid.Empty)
        {
            return null;
        }

        var organizacao = await organizacaoRepositorio.BuscarPorIdAsync(organizacaoId, cancellationToken);
        return organizacao is null
            ? null
            : new ConsultarOrganizacaoAtualResultado(
                organizacao.Id, organizacao.Nome.Valor, organizacao.EnderecoPrestador?.Descricao, organizacao.FusoHorario?.Identificador);
    }
}
