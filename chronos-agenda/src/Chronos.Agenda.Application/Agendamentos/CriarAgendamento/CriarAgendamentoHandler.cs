using Chronos.Agenda.Application.Agendamentos.Contratos;
using Chronos.Agenda.Application.Agendamentos.Erros;
using Chronos.Agenda.Application.Compartilhado.Contratos;
using Chronos.Agenda.Application.Disponibilidades.Contratos;
using Chronos.Agenda.Application.Organizacoes.Contratos;
using Chronos.Agenda.Application.Organizacoes.Erros;
using Chronos.Agenda.Application.Profissionais.Contratos;
using Chronos.Agenda.Application.Profissionais.Erros;
using Chronos.Agenda.Application.Servicos.Contratos;
using Chronos.Agenda.Application.Servicos.Erros;
using Chronos.Agenda.Domain.Agendamentos.Entidades;
using Chronos.Agenda.Domain.Agendamentos.ObjetosValor;
using Chronos.Agenda.Domain.Agendamentos.Servicos;
using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Exceptions;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Compartilhado.Resultados;
using Chronos.Agenda.Domain.Disponibilidades.ObjetosValor;
using Chronos.Agenda.Domain.Disponibilidades.Servicos;
using Chronos.Agenda.Domain.Organizacoes.Entidades;
using Chronos.Agenda.Domain.Profissionais.Entidades;
using Chronos.Agenda.Domain.Servicos.Entidades;
using Chronos.Agenda.Domain.Servicos.Enums;

namespace Chronos.Agenda.Application.Agendamentos.CriarAgendamento;

/// <summary>
/// Cria um novo agendamento (UC04): valida profissional, serviço e seu
/// vínculo (RN04), calcula o período pela duração vigente do serviço (RN05),
/// resolve e valida o local de atendimento (RN06), verifica se o período cabe
/// na disponibilidade do profissional (RN07) e rejeita conflito com outro
/// agendamento ativo (RN02).
/// </summary>
/// <example><code>
/// var resultado = await handler.ExecutarAsync(
///     new CriarAgendamentoComando(profissionalId, servicoId, "Maria Silva", TipoPessoaAtendida.Paciente, inicio, null),
///     cancellationToken);
/// </code></example>
public sealed class CriarAgendamentoHandler(
    IAgendamentoRepositorio agendamentoRepositorio,
    IProfissionalRepositorio profissionalRepositorio,
    IServicoRepositorio servicoRepositorio,
    IOrganizacaoRepositorio organizacaoRepositorio,
    IDisponibilidadeSemanalRepositorio disponibilidadeRepositorio,
    IContextoUsuario contextoUsuario,
    IUnidadeDeTrabalho unidadeDeTrabalho,
    IProvedorDataHora provedorDataHora)
{
    public async Task<Resultado<CriarAgendamentoResultado>> ExecutarAsync(CriarAgendamentoComando comando, CancellationToken cancellationToken)
    {
        var organizacaoId = contextoUsuario.ObterOrganizacaoId();

        var preparadoResultado = await PrepararAsync(organizacaoId, comando, cancellationToken);
        if (preparadoResultado.Falhou)
        {
            return Resultado<CriarAgendamentoResultado>.Falha(preparadoResultado.Erro!);
        }

        var preparado = preparadoResultado.Valor;
        var regrasResultado = await ValidarRegrasAsync(organizacaoId, preparado, cancellationToken);
        if (regrasResultado.Falhou)
        {
            return Resultado<CriarAgendamentoResultado>.Falha(regrasResultado.Erro!);
        }

        var agendamento = Agendamento.Criar(
            organizacaoId, preparado.Profissional.Id, preparado.Servico.Id, preparado.Servico.Nome.Valor,
            preparado.PessoaAtendida, preparado.Periodo, preparado.Servico.Preco, preparado.Local, provedorDataHora);
        await agendamentoRepositorio.AdicionarAsync(agendamento, cancellationToken);
        await unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);
        return Resultado<CriarAgendamentoResultado>.Ok(new CriarAgendamentoResultado(agendamento.Id));
    }

    private async Task<Resultado<AgendamentoPreparado>> PrepararAsync(Guid organizacaoId, CriarAgendamentoComando comando, CancellationToken cancellationToken)
    {
        var referenciasResultado = await ResolverReferenciasAsync(organizacaoId, comando, cancellationToken);
        if (referenciasResultado.Falhou)
        {
            return Resultado<AgendamentoPreparado>.Falha(referenciasResultado.Erro!);
        }

        var (profissional, servico, organizacao) = referenciasResultado.Valor;
        var pessoaAtendidaResultado = CriarPessoaAtendida(comando);
        if (pessoaAtendidaResultado.Falhou)
        {
            return Resultado<AgendamentoPreparado>.Falha(pessoaAtendidaResultado.Erro!);
        }

        var periodo = PeriodoAgendamento.APartirDaDuracao(comando.Inicio.UtcDateTime, servico.Duracao);
        var localResultado = ResolverELocalizar(servico, organizacao, comando.EnderecoPessoaAtendida);
        if (localResultado.Falhou)
        {
            return Resultado<AgendamentoPreparado>.Falha(localResultado.Erro!);
        }

        return Resultado<AgendamentoPreparado>.Ok(
            new AgendamentoPreparado(profissional, servico, organizacao, pessoaAtendidaResultado.Valor, periodo, localResultado.Valor));
    }

    private async Task<Resultado<(Profissional, Servico, Organizacao)>> ResolverReferenciasAsync(
        Guid organizacaoId, CriarAgendamentoComando comando, CancellationToken cancellationToken)
    {
        var profissional = await profissionalRepositorio.BuscarPorIdAsync(organizacaoId, comando.ProfissionalId, cancellationToken);
        if (profissional is null)
        {
            return Resultado<(Profissional, Servico, Organizacao)>.Falha(ProfissionalErros.NaoEncontrado(organizacaoId, comando.ProfissionalId));
        }

        var servico = await servicoRepositorio.BuscarPorIdAsync(organizacaoId, comando.ServicoId, cancellationToken);
        if (servico is null)
        {
            return Resultado<(Profissional, Servico, Organizacao)>.Falha(ServicoErros.NaoEncontrado(organizacaoId, comando.ServicoId));
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

    private static Resultado<PessoaAtendida> CriarPessoaAtendida(CriarAgendamentoComando comando)
    {
        try
        {
            return Resultado<PessoaAtendida>.Ok(new PessoaAtendida(new Nome(comando.NomePessoaAtendida), comando.TipoPessoaAtendida));
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

    private async Task<Resultado> ValidarRegrasAsync(Guid organizacaoId, AgendamentoPreparado preparado, CancellationToken cancellationToken)
    {
        var disponibilidadeResultado = await VerificarDisponibilidadeAsync(organizacaoId, preparado, cancellationToken);
        if (disponibilidadeResultado.Falhou)
        {
            return disponibilidadeResultado;
        }

        var sobrepostos = await agendamentoRepositorio.BuscarAtivosSobrepostosAsync(
            organizacaoId, preparado.Profissional.Id, preparado.Periodo, cancellationToken);
        return sobrepostos.Count == 0
            ? Resultado.Ok()
            : Resultado.Falha(AgendamentoErros.ConflitoDeAgenda(preparado.Profissional.Id));
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
