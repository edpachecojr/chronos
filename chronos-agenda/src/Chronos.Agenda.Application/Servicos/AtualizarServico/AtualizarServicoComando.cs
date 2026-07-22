using Chronos.Agenda.Domain.Servicos.Enums;

namespace Chronos.Agenda.Application.Servicos.AtualizarServico;

/// <summary>Dados para atualizar a configuração comercial de um serviço existente (UC03).</summary>
/// <example><code>
/// var comando = new AtualizarServicoComando(servicoId, "Consulta de retorno", TimeSpan.FromMinutes(30), 180m, TipoAtendimento.Online);
/// </code></example>
public sealed record AtualizarServicoComando(Guid ServicoId, string Nome, TimeSpan Duracao, decimal Preco, TipoAtendimento TipoAtendimento);
