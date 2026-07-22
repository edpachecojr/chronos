using Chronos.Agenda.Domain.Servicos.Enums;

namespace Chronos.Agenda.Application.Servicos.CriarServico;

/// <summary>Dados para criar um novo serviço oferecido por um profissional (UC03).</summary>
/// <example><code>
/// var comando = new CriarServicoComando(profissionalId, "Consulta inicial", TimeSpan.FromMinutes(50), 250m, TipoAtendimento.Online);
/// </code></example>
public sealed record CriarServicoComando(Guid ProfissionalId, string Nome, TimeSpan Duracao, decimal Preco, TipoAtendimento TipoAtendimento);
