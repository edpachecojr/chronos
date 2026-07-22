using Chronos.Agenda.Application.Agendamentos.Contratos;
using Chronos.Agenda.Application.Agendamentos.Erros;
using Chronos.Agenda.Application.Disponibilidades.Contratos;
using Chronos.Agenda.Application.Organizacoes.Contratos;
using Chronos.Agenda.Application.Organizacoes.Erros;
using Chronos.Agenda.Application.Profissionais.Contratos;
using Chronos.Agenda.Application.Profissionais.Erros;
using Chronos.Agenda.Application.Servicos.Contratos;
using Chronos.Agenda.Application.Servicos.Erros;
using Chronos.Agenda.Domain.Agendamentos.ObjetosValor;
using Chronos.Agenda.Domain.Agendamentos.Servicos;
using Chronos.Agenda.Domain.Compartilhado.Exceptions;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Compartilhado.Resultados;
using Chronos.Agenda.Domain.Disponibilidades.ObjetosValor;
using Chronos.Agenda.Domain.Disponibilidades.Servicos;
using Chronos.Agenda.Domain.Organizacoes.Entidades;
using Chronos.Agenda.Domain.Profissionais.Entidades;
using Chronos.Agenda.Domain.Servicos.Entidades;
using Chronos.Agenda.Domain.Servicos.Enums;

namespace Chronos.Agenda.Application.Agendamentos;

/// <summary>
/// Resolve profissional, serviço, organização, pessoa atendida, período e local de um agendamento, e valida
/// disponibilidade (RN07) e conflito de agenda (RN02). Compartilhado entre criar e reagendar (UC04, UC05), que
/// fazem a mesma preparação e as mesmas validações antes de persistir.
/// </summary>
/// <example><code>
/// var preparador = new AgendamentoPreparador(profissionalRepositorio, servicoRepositorio, organizacaoRepositorio, disponibilidadeRepositorio, agendamentoRepositorio);
/// var preparado = await preparador.PrepararAsync(organizacaoId, comando, cancellationToken);
/// </code></example>
internal sealed class AgendamentoPreparador(
    IProfissionalRepositorio profissionalRepositorio,
    IServicoRepositorio servicoRepositorio,
    IOrganizacaoRepositorio organizacaoRepositorio,
    IDisponibilidadeSemanalRepositorio disponibilidadeRepositorio,
    IAgendamentoRepositorio agendamentoRepositorio)
{
    public async Task<Resultado<AgendamentoPreparado>> PrepararAsync(Guid organizacaoId, IDadosAgendamento dados, CancellationToken cancellationToken)
    {
        var referenciasResultado = await ResolverReferenciasAsync(organizacaoId, dados, cancellationToken);
        if (referenciasResultado.Falhou)
        {
            return Resultado<AgendamentoPreparado>.Falha(referenciasResultado.Erro!);
        }

        var (profissional, servico, organizacao) = referenciasResultado.Valor;
        var pessoaAtendidaResultado = CriarPessoaAtendida(dados);
        if (pessoaAtendidaResultado.Falhou)
        {
            return Resultado<AgendamentoPreparado>.Falha(pessoaAtendidaResultado.Erro!);
        }

        var periodo = PeriodoAgendamento.APartirDaDuracao(dados.Inicio.UtcDateTime, servico.Duracao);
        var localResultado = ResolverELocalizar(servico, organizacao, dados.EnderecoPessoaAtendida);
        if (localResultado.Falhou)
        {
            return Resultado<AgendamentoPreparado>.Falha(localResultado.Erro!);
        }

        return Resultado<AgendamentoPreparado>.Ok(
            new AgendamentoPreparado(profissional, servico, organizacao, pessoaAtendidaResultado.Valor, periodo, localResultado.Valor));
    }

    /// <summary><paramref name="excluirAgendamentoId"/> remove o próprio agendamento da busca de conflito ao
    /// reagendar (UC05); passe <c>null</c> ao criar um agendamento novo (UC04).</summary>
    public async Task<Resultado> ValidarRegrasAsync(
        Guid organizacaoId, AgendamentoPreparado preparado, Guid? excluirAgendamentoId, CancellationToken cancellationToken)
    {
        var disponibilidadeResultado = await VerificarDisponibilidadeAsync(organizacaoId, preparado, cancellationToken);
        if (disponibilidadeResultado.Falhou)
        {
            return disponibilidadeResultado;
        }

        var sobrepostos = await agendamentoRepositorio.BuscarAtivosSobrepostosAsync(
            organizacaoId, preparado.Profissional.Id, preparado.Periodo, excluirAgendamentoId, cancellationToken);
        return sobrepostos.Count == 0
            ? Resultado.Ok()
            : Resultado.Falha(AgendamentoErros.ConflitoDeAgenda(preparado.Profissional.Id));
    }

    private async Task<Resultado<(Profissional, Servico, Organizacao)>> ResolverReferenciasAsync(
        Guid organizacaoId, IDadosAgendamento dados, CancellationToken cancellationToken)
    {
        var profissional = await profissionalRepositorio.BuscarPorIdAsync(organizacaoId, dados.ProfissionalId, cancellationToken);
        if (profissional is null)
        {
            return Resultado<(Profissional, Servico, Organizacao)>.Falha(ProfissionalErros.NaoEncontrado(organizacaoId, dados.ProfissionalId));
        }

        var servico = await servicoRepositorio.BuscarPorIdAsync(organizacaoId, dados.ServicoId, cancellationToken);
        if (servico is null)
        {
            return Resultado<(Profissional, Servico, Organizacao)>.Falha(ServicoErros.NaoEncontrado(organizacaoId, dados.ServicoId));
        }

        if (servico.ProfissionalId != profissional.Id)
        {
            return Resultado<(Profissional, Servico, Organizacao)>.Falha(AgendamentoErros.ServicoNaoPertenceAoProfissional(servico.Id, profissional.Id));
        }

        var organizacao = await organizacaoRepositorio.BuscarPorIdAsync(organizacaoId, cancellationToken);
        if (organizacao is null)
        {
            return Resultado<(Profissional, Servico, Organizacao)>.Falha(OrganizacaoErros.NaoEncontrada(organizacaoId));
        }

        if (organizacao.FusoHorario is null)
        {
            return Resultado<(Profissional, Servico, Organizacao)>.Falha(AgendamentoErros.PerfilOperacionalNaoConfigurado(organizacaoId));
        }

        return Resultado<(Profissional, Servico, Organizacao)>.Ok((profissional, servico, organizacao));
    }

    private static Resultado<PessoaAtendida> CriarPessoaAtendida(IDadosAgendamento dados)
    {
        try
        {
            return Resultado<PessoaAtendida>.Ok(new PessoaAtendida(new Nome(dados.NomePessoaAtendida), dados.TipoPessoaAtendida));
        }
        catch (NomeInvalidoException excecao)
        {
            return Resultado<PessoaAtendida>.Falha(AgendamentoErros.NomePessoaAtendidaInvalido(excecao.Message));
        }
    }

    private static Resultado<LocalAtendimento> ResolverELocalizar(Servico servico, Organizacao organizacao, string? enderecoPessoaAtendida)
    {
        var localResultado = servico.TipoAtendimento switch
        {
            TipoAtendimento.Online => Resultado<LocalAtendimento>.Ok(LocalAtendimento.Online()),
            TipoAtendimento.Domiciliar => ResolverLocalDomiciliar(enderecoPessoaAtendida),
            TipoAtendimento.NoEnderecoDoPrestador => ResolverLocalNoEnderecoDoPrestador(organizacao),
            _ => throw new ArgumentOutOfRangeException(nameof(servico), servico.TipoAtendimento, "Tipo de atendimento do serviço desconhecido."),
        };
        if (localResultado.Falhou)
        {
            return localResultado;
        }

        var compatibilidadeResultado = VerificadorCompatibilidadeLocal.Verificar(servico.TipoAtendimento, localResultado.Valor);
        return compatibilidadeResultado.Falhou ? Resultado<LocalAtendimento>.Falha(compatibilidadeResultado.Erro!) : localResultado;
    }

    private static Resultado<LocalAtendimento> ResolverLocalDomiciliar(string? enderecoPessoaAtendida)
    {
        if (string.IsNullOrWhiteSpace(enderecoPessoaAtendida))
        {
            return Resultado<LocalAtendimento>.Falha(AgendamentoErros.EnderecoObrigatorioAusente);
        }

        try
        {
            return Resultado<LocalAtendimento>.Ok(LocalAtendimento.Domiciliar(new EnderecoAtendimento(enderecoPessoaAtendida)));
        }
        catch (EnderecoAtendimentoInvalidoException excecao)
        {
            return Resultado<LocalAtendimento>.Falha(AgendamentoErros.EnderecoInvalido(excecao.Message));
        }
    }

    private static Resultado<LocalAtendimento> ResolverLocalNoEnderecoDoPrestador(Organizacao organizacao)
    {
        return organizacao.EnderecoPrestador is null
            ? Resultado<LocalAtendimento>.Falha(AgendamentoErros.PerfilOperacionalNaoConfigurado(organizacao.Id))
            : Resultado<LocalAtendimento>.Ok(LocalAtendimento.NoEnderecoDoPrestador(organizacao.EnderecoPrestador));
    }

    private async Task<Resultado> VerificarDisponibilidadeAsync(Guid organizacaoId, AgendamentoPreparado preparado, CancellationToken cancellationToken)
    {
        var fusoHorario = preparado.Organizacao.FusoHorario!;
        var inicioLocal = fusoHorario.ConverterParaLocal(preparado.Periodo.InicioUtc);
        var fimLocal = fusoHorario.ConverterParaLocal(preparado.Periodo.FimUtc);
        if (inicioLocal.Date != fimLocal.Date)
        {
            return Resultado.Falha(AgendamentoErros.PeriodoAtravessaMeiaNoite(inicioLocal, fimLocal));
        }

        var diaDaSemana = inicioLocal.DayOfWeek;
        var janela = new JanelaHorario(TimeOnly.FromDateTime(inicioLocal.DateTime), TimeOnly.FromDateTime(fimLocal.DateTime));
        var disponibilidades = await disponibilidadeRepositorio.BuscarPorProfissionalEDiaAsync(
            organizacaoId, preparado.Profissional.Id, diaDaSemana, cancellationToken);
        return VerificadorDisponibilidade.Verificar(diaDaSemana, janela, disponibilidades);
    }
}
