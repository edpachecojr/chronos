using Chronos.Agenda.Application.Compartilhado;
using Chronos.Agenda.Application.Servicos.ListarServicos;
using Chronos.Agenda.Application.Tests.Compartilhado.Fakes;
using Chronos.Agenda.Application.Tests.Profissionais.Fakes;
using Chronos.Agenda.Application.Tests.Servicos.Fakes;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Profissionais.Entidades;
using Chronos.Agenda.Domain.Servicos.Entidades;
using Chronos.Agenda.Domain.Servicos.Enums;
using Chronos.Agenda.Domain.Servicos.ObjetosValor;

namespace Chronos.Agenda.Application.Tests.Servicos.ListarServicos;

public class ListarServicosHandlerTests
{
    private static readonly DateTime CriadoEmUtc = new(2026, 7, 22, 12, 0, 0, DateTimeKind.Utc);

    private readonly Guid _organizacaoId = Guid.NewGuid();
    private readonly FakeServicoRepositorio _servicoRepositorio = new();
    private readonly FakeProfissionalRepositorio _profissionalRepositorio = new();
    private readonly FakeProvedorDataHora _provedorDataHora = new(CriadoEmUtc);
    private readonly Profissional _profissional;
    private readonly ListarServicosHandler _handler;

    public ListarServicosHandlerTests()
    {
        _profissional = Profissional.Criar(_organizacaoId, new Nome("Dra. Ana Souza"), _provedorDataHora);
        _profissionalRepositorio.AdicionarAsync(_profissional, CancellationToken.None).GetAwaiter().GetResult();
        _handler = new ListarServicosHandler(_servicoRepositorio, new ContextoUsuario(Guid.NewGuid(), _organizacaoId));
    }

    [Fact]
    public async Task ExecutarAsync_retorna_servicos_do_profissional_ordenados_por_nome()
    {
        var corte = CriarServico("Corte de cabelo", TimeSpan.FromMinutes(30), 60m, TipoAtendimento.NoEnderecoDoPrestador);
        var consulta = CriarServico("Consulta inicial", TimeSpan.FromMinutes(50), 250m, TipoAtendimento.Online);
        await _servicoRepositorio.AdicionarAsync(corte, CancellationToken.None);
        await _servicoRepositorio.AdicionarAsync(consulta, CancellationToken.None);

        var resultado = await _handler.ExecutarAsync(new ListarServicosConsulta(_profissional.Id), CancellationToken.None);

        Assert.Equal(2, resultado.Count);
        Assert.Equal(["Consulta inicial", "Corte de cabelo"], resultado.Select(r => r.Nome));
        var primeiro = resultado.First();
        Assert.Equal(consulta.Id, primeiro.ServicoId);
        Assert.Equal(TimeSpan.FromMinutes(50), primeiro.Duracao);
        Assert.Equal(250m, primeiro.Preco);
        Assert.Equal(TipoAtendimento.Online, primeiro.TipoAtendimento);
    }

    [Fact]
    public async Task ExecutarAsync_com_profissional_sem_servicos_retorna_vazio()
    {
        var resultado = await _handler.ExecutarAsync(new ListarServicosConsulta(_profissional.Id), CancellationToken.None);

        Assert.Empty(resultado);
    }

    [Fact]
    public async Task ExecutarAsync_nao_retorna_servicos_de_outro_profissional()
    {
        var outroProfissional = Profissional.Criar(_organizacaoId, new Nome("Dr. João Lima"), _provedorDataHora);
        await _profissionalRepositorio.AdicionarAsync(outroProfissional, CancellationToken.None);
        var servicoDoOutro = CriarServico("Massagem", TimeSpan.FromMinutes(60), 150m, TipoAtendimento.NoEnderecoDoPrestador, outroProfissional.Id);
        await _servicoRepositorio.AdicionarAsync(servicoDoOutro, CancellationToken.None);

        var resultado = await _handler.ExecutarAsync(new ListarServicosConsulta(_profissional.Id), CancellationToken.None);

        Assert.Empty(resultado);
    }

    private Servico CriarServico(string nome, TimeSpan duracao, decimal preco, TipoAtendimento tipoAtendimento, Guid? profissionalId = null)
    {
        return Servico.Criar(
            _organizacaoId, profissionalId ?? _profissional.Id, new NomeServico(nome), new DuracaoServico(duracao), new PrecoServico(preco),
            tipoAtendimento, _provedorDataHora);
    }
}
