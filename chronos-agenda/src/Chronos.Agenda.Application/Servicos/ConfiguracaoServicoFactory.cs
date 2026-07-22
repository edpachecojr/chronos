using Chronos.Agenda.Application.Servicos.Erros;
using Chronos.Agenda.Domain.Compartilhado.Resultados;
using Chronos.Agenda.Domain.Servicos.Exceptions;
using Chronos.Agenda.Domain.Servicos.ObjetosValor;

namespace Chronos.Agenda.Application.Servicos;

/// <summary>Constrói os objetos de valor comerciais de um serviço a partir de dados primitivos, convertendo as
/// exceções de domínio em erros esperados da aplicação. Reaproveitado pelos casos de uso de criar e atualizar
/// serviço, que validam os mesmos três campos.</summary>
internal static class ConfiguracaoServicoFactory
{
    /// <example><code>var resultado = ConfiguracaoServicoFactory.Criar(comando.Nome, comando.Duracao, comando.Preco);</code></example>
    public static Resultado<ConfiguracaoServico> Criar(string nome, TimeSpan duracao, decimal preco)
    {
        try
        {
            return Resultado<ConfiguracaoServico>.Ok(new ConfiguracaoServico(new NomeServico(nome), new DuracaoServico(duracao), new PrecoServico(preco)));
        }
        catch (NomeServicoInvalidoException excecao)
        {
            return Resultado<ConfiguracaoServico>.Falha(ServicoErros.NomeInvalido(excecao.Message));
        }
        catch (DuracaoServicoInvalidaException excecao)
        {
            return Resultado<ConfiguracaoServico>.Falha(ServicoErros.DuracaoInvalida(excecao.Message));
        }
        catch (PrecoServicoInvalidoException excecao)
        {
            return Resultado<ConfiguracaoServico>.Falha(ServicoErros.PrecoInvalido(excecao.Message));
        }
    }
}

/// <summary>Conjunto validado dos objetos de valor comerciais de um serviço.</summary>
internal sealed record ConfiguracaoServico(NomeServico Nome, DuracaoServico Duracao, PrecoServico Preco);
