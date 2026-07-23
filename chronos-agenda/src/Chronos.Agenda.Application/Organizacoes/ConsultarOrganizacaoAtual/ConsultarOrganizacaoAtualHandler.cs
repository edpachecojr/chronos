using Chronos.Agenda.Application.Disponibilidades.Contratos;
using Chronos.Agenda.Application.Organizacoes.Contratos;
using Chronos.Agenda.Application.Profissionais.Contratos;
using Chronos.Agenda.Application.Servicos.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Profissionais.Entidades;

namespace Chronos.Agenda.Application.Organizacoes.ConsultarOrganizacaoAtual;

/// <summary>
/// Resolve a organização vinculada ao usuário autenticado, para que o
/// frontend saiba se deve encaminhá-lo ao onboarding (UC01) ou ao produto.
/// Não é uma falha esperada o usuário ainda não pertencer a uma organização
/// — por isso o resultado é nulo nesse caso, em vez de modelado como erro do
/// Result Pattern. Também informa o progresso de onboarding (perfil
/// operacional, disponibilidade e serviço), para o frontend retomar o
/// wizard na etapa correta sem precisar de um campo de progresso à parte.
/// </summary>
/// <example><code>var resultado = await handler.ExecutarAsync(cancellationToken);</code></example>
public sealed class ConsultarOrganizacaoAtualHandler(
    IOrganizacaoRepositorio organizacaoRepositorio,
    IProfissionalRepositorio profissionalRepositorio,
    IDisponibilidadeSemanalRepositorio disponibilidadeSemanalRepositorio,
    IServicoRepositorio servicoRepositorio,
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
        if (organizacao is null)
        {
            return null;
        }

        var (possuiDisponibilidade, possuiServico) = await ConsultarProgressoAsync(organizacaoId, cancellationToken);
        return new ConsultarOrganizacaoAtualResultado(
            organizacao.Id,
            organizacao.Nome.Valor,
            organizacao.EnderecoPrestador?.Descricao,
            organizacao.FusoHorario?.Identificador,
            possuiDisponibilidade,
            possuiServico);
    }

    private async Task<(bool PossuiDisponibilidade, bool PossuiServico)> ConsultarProgressoAsync(
        Guid organizacaoId, CancellationToken cancellationToken)
    {
        var profissional = await BuscarProfissionalInicialAsync(organizacaoId, cancellationToken);
        if (profissional is null)
        {
            return (false, false);
        }

        var disponibilidades = await disponibilidadeSemanalRepositorio.BuscarPorProfissionalAsync(
            organizacaoId, profissional.Id, cancellationToken);
        var servicos = await servicoRepositorio.BuscarPorProfissionalAsync(
            organizacaoId, profissional.Id, cancellationToken);

        return (disponibilidades.Count > 0, servicos.Count > 0);
    }

    private async Task<Profissional?> BuscarProfissionalInicialAsync(Guid organizacaoId, CancellationToken cancellationToken)
    {
        var profissionais = await profissionalRepositorio.BuscarPorOrganizacaoAsync(organizacaoId, cancellationToken);
        return profissionais.FirstOrDefault();
    }
}
