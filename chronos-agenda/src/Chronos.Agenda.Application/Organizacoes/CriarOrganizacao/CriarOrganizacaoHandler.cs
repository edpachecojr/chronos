using Chronos.Agenda.Application.Compartilhado;
using Chronos.Agenda.Application.Compartilhado.Contratos;
using Chronos.Agenda.Application.Organizacoes.Contratos;
using Chronos.Agenda.Application.Organizacoes.Erros;
using Chronos.Agenda.Application.Profissionais.Contratos;
using Chronos.Agenda.Application.Profissionais.Erros;
using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Exceptions;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Compartilhado.Resultados;
using Chronos.Agenda.Domain.Organizacoes.Entidades;
using Chronos.Agenda.Domain.Organizacoes.Exceptions;
using Chronos.Agenda.Domain.Organizacoes.ObjetosValor;
using Chronos.Agenda.Domain.Profissionais.Entidades;

namespace Chronos.Agenda.Application.Organizacoes.CriarOrganizacao;

/// <summary>
/// Executa o onboarding de uma organização (UC01): cria a organização e seu
/// primeiro profissional na mesma transação e registra o vínculo entre o
/// usuário autenticado que fez o onboarding e a organização criada, com o
/// papel de proprietário (<see cref="IMembroOrganizacaoRepositorio"/>, ADR
/// 0003) — quem faz o onboarding sempre passa a administrar a organização
/// criada.
/// </summary>
/// <example><code>
/// var resultado = await handler.ExecutarAsync(
///     new CriarOrganizacaoComando(usuarioId, "Clínica Bem-Estar", "Dra. Ana Souza"),
///     cancellationToken);
/// </code></example>
public sealed class CriarOrganizacaoHandler(
    IOrganizacaoRepositorio organizacaoRepositorio,
    IProfissionalRepositorio profissionalRepositorio,
    IMembroOrganizacaoRepositorio membroOrganizacaoRepositorio,
    IUnidadeDeTrabalho unidadeDeTrabalho,
    IProvedorDataHora provedorDataHora)
{
    public async Task<Resultado<CriarOrganizacaoResultado>> ExecutarAsync(
        CriarOrganizacaoComando comando, CancellationToken cancellationToken)
    {
        var organizacaoResultado = CriarOrganizacao(comando.Nome);
        if (organizacaoResultado.Falhou)
        {
            return Resultado<CriarOrganizacaoResultado>.Falha(organizacaoResultado.Erro!);
        }

        var organizacao = organizacaoResultado.Valor;
        var profissionalResultado = CriarProfissionalInicial(organizacao.Id, comando.NomeProfissionalInicial);
        if (profissionalResultado.Falhou)
        {
            return Resultado<CriarOrganizacaoResultado>.Falha(profissionalResultado.Erro!);
        }

        var profissional = profissionalResultado.Valor;
        await PersistirAsync(organizacao, profissional, comando.UsuarioId, cancellationToken);

        return Resultado<CriarOrganizacaoResultado>.Ok(new CriarOrganizacaoResultado(organizacao.Id, profissional.Id));
    }

    private Resultado<Organizacao> CriarOrganizacao(string nome)
    {
        try
        {
            return Resultado<Organizacao>.Ok(Organizacao.Criar(new NomeOrganizacao(nome), provedorDataHora));
        }
        catch (NomeOrganizacaoInvalidoException excecao)
        {
            return Resultado<Organizacao>.Falha(OrganizacaoErros.NomeInvalido(excecao.Message));
        }
    }

    private Resultado<Profissional> CriarProfissionalInicial(Guid organizacaoId, string nome)
    {
        try
        {
            return Resultado<Profissional>.Ok(Profissional.Criar(organizacaoId, new Nome(nome), provedorDataHora));
        }
        catch (NomeInvalidoException excecao)
        {
            return Resultado<Profissional>.Falha(ProfissionalErros.NomeInvalido(excecao.Message));
        }
    }

    private async Task PersistirAsync(Organizacao organizacao, Profissional profissional, Guid usuarioId, CancellationToken cancellationToken)
    {
        await organizacaoRepositorio.AdicionarAsync(organizacao, cancellationToken);
        await profissionalRepositorio.AdicionarAsync(profissional, cancellationToken);
        await membroOrganizacaoRepositorio.AdicionarAsync(usuarioId, organizacao.Id, PapelMembroOrganizacao.Proprietario, cancellationToken);
        await unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);
    }
}
