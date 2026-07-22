using Chronos.Agenda.Application.Compartilhado.Contratos;
using Chronos.Agenda.Application.Disponibilidades.Contratos;
using Chronos.Agenda.Application.Disponibilidades.Erros;
using Chronos.Agenda.Application.Profissionais.Contratos;
using Chronos.Agenda.Application.Profissionais.Erros;
using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Resultados;

namespace Chronos.Agenda.Application.Disponibilidades.RemoverDisponibilidade;

/// <summary>
/// Remove uma disponibilidade semanal de um profissional (UC02). Não há
/// política de retenção para configuração de expediente, diferente do
/// agendamento: a exclusão é direta.
/// </summary>
/// <example><code>
/// var resultado = await handler.ExecutarAsync(
///     new RemoverDisponibilidadeComando(profissionalId, disponibilidadeId), cancellationToken);
/// </code></example>
public sealed class RemoverDisponibilidadeHandler(
    IDisponibilidadeSemanalRepositorio disponibilidadeRepositorio,
    IProfissionalRepositorio profissionalRepositorio,
    IContextoUsuario contextoUsuario,
    IUnidadeDeTrabalho unidadeDeTrabalho)
{
    public async Task<Resultado> ExecutarAsync(RemoverDisponibilidadeComando comando, CancellationToken cancellationToken)
    {
        var organizacaoId = contextoUsuario.ObterOrganizacaoId();

        var profissional = await profissionalRepositorio.BuscarPorIdAsync(organizacaoId, comando.ProfissionalId, cancellationToken);
        if (profissional is null)
        {
            return Resultado.Falha(ProfissionalErros.NaoEncontrado(organizacaoId, comando.ProfissionalId));
        }

        var todasDoProfissional = await disponibilidadeRepositorio.BuscarPorProfissionalAsync(organizacaoId, profissional.Id, cancellationToken);
        var alvo = todasDoProfissional.SingleOrDefault(d => d.Id == comando.DisponibilidadeId);
        if (alvo is null)
        {
            return Resultado.Falha(DisponibilidadeErros.NaoEncontrada(comando.DisponibilidadeId));
        }

        await disponibilidadeRepositorio.RemoverAsync(alvo, cancellationToken);
        await unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);
        return Resultado.Ok();
    }
}
