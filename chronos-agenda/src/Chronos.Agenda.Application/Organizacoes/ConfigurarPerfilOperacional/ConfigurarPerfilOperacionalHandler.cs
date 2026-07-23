using Chronos.Agenda.Application.Compartilhado.Contratos;
using Chronos.Agenda.Application.Organizacoes.Contratos;
using Chronos.Agenda.Application.Organizacoes.Erros;
using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Exceptions;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Compartilhado.Resultados;
using Chronos.Agenda.Domain.Organizacoes.Exceptions;
using Chronos.Agenda.Domain.Organizacoes.ObjetosValor;

namespace Chronos.Agenda.Application.Organizacoes.ConfigurarPerfilOperacional;

/// <summary>
/// Configura o perfil operacional (endereço do prestador e fuso horário) da
/// organização corrente, expondo <c>Organizacao.ConfigurarPerfilOperacional</c>
/// à aplicação. Destrava UC04, UC05 e UC07: sem fuso horário configurado, a
/// criação, o reagendamento e a consulta de agenda falham com
/// <c>Agendamento.PerfilOperacionalNaoConfigurado</c> (ver
/// docs/backlog/roadmap-casos-de-uso.md, bloqueador crítico).
/// </summary>
/// <example><code>
/// var resultado = await handler.ExecutarAsync(
///     new ConfigurarPerfilOperacionalComando("Av. Central, 20", "America/Sao_Paulo"),
///     cancellationToken);
/// </code></example>
public sealed class ConfigurarPerfilOperacionalHandler(
    IOrganizacaoRepositorio organizacaoRepositorio,
    IContextoUsuario contextoUsuario,
    IUnidadeDeTrabalho unidadeDeTrabalho,
    IProvedorDataHora provedorDataHora)
{
    public async Task<Resultado> ExecutarAsync(ConfigurarPerfilOperacionalComando comando, CancellationToken cancellationToken)
    {
        var organizacaoId = contextoUsuario.ObterOrganizacaoId();
        var organizacao = await organizacaoRepositorio.BuscarPorIdAsync(organizacaoId, cancellationToken);
        if (organizacao is null)
        {
            return Resultado.Falha(OrganizacaoErros.NaoEncontrada(organizacaoId));
        }

        var enderecoResultado = CriarEndereco(comando.EnderecoPrestador);
        if (enderecoResultado.Falhou)
        {
            return Resultado.Falha(enderecoResultado.Erro!);
        }

        var fusoHorarioResultado = CriarFusoHorario(comando.FusoHorario);
        if (fusoHorarioResultado.Falhou)
        {
            return Resultado.Falha(fusoHorarioResultado.Erro!);
        }

        organizacao.ConfigurarPerfilOperacional(enderecoResultado.Valor, fusoHorarioResultado.Valor, provedorDataHora);
        await organizacaoRepositorio.AtualizarAsync(organizacao, cancellationToken);
        await unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);
        return Resultado.Ok();
    }

    private static Resultado<EnderecoAtendimento?> CriarEndereco(string? enderecoPrestador)
    {
        if (string.IsNullOrWhiteSpace(enderecoPrestador))
        {
            return Resultado<EnderecoAtendimento?>.Ok(null);
        }

        try
        {
            return Resultado<EnderecoAtendimento?>.Ok(new EnderecoAtendimento(enderecoPrestador));
        }
        catch (EnderecoAtendimentoInvalidoException excecao)
        {
            return Resultado<EnderecoAtendimento?>.Falha(OrganizacaoErros.EnderecoInvalido(excecao.Message));
        }
    }

    private static Resultado<FusoHorario> CriarFusoHorario(string fusoHorario)
    {
        try
        {
            return Resultado<FusoHorario>.Ok(new FusoHorario(fusoHorario));
        }
        catch (FusoHorarioInvalidoException excecao)
        {
            return Resultado<FusoHorario>.Falha(OrganizacaoErros.FusoHorarioInvalido(excecao.Message));
        }
    }
}
