using Chronos.Agenda.Application.Profissionais.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Contratos;

namespace Chronos.Agenda.Application.Profissionais.ListarProfissionais;

/// <summary>
/// Lista os profissionais vinculados à organização corrente. Leitura pura, sem falha esperada: o MVP não tem
/// caso de uso de convite/adição de profissional, então a lista tem exatamente um item após o onboarding (UC01).
/// </summary>
/// <example><code>var profissionais = await handler.ExecutarAsync(cancellationToken);</code></example>
public sealed class ListarProfissionaisHandler(IProfissionalRepositorio profissionalRepositorio, IContextoUsuario contextoUsuario)
{
    public async Task<IReadOnlyCollection<ProfissionalResumo>> ExecutarAsync(CancellationToken cancellationToken)
    {
        var organizacaoId = contextoUsuario.ObterOrganizacaoId();
        var profissionais = await profissionalRepositorio.BuscarPorOrganizacaoAsync(organizacaoId, cancellationToken);

        return profissionais
            .Select(profissional => new ProfissionalResumo(profissional.Id, profissional.Nome.Valor))
            .OrderBy(resumo => resumo.Nome)
            .ToList();
    }
}
