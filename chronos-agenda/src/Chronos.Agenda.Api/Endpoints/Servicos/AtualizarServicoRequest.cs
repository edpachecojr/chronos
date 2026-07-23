using Chronos.Agenda.Domain.Servicos.Enums;

namespace Chronos.Agenda.Api.Endpoints.Servicos;

/// <summary>Corpo da requisição para atualizar a configuração comercial de um serviço existente (UC03). O
/// identificador do serviço vem da rota, não do corpo.</summary>
public sealed record AtualizarServicoRequest(string Nome, TimeSpan Duracao, decimal Preco, TipoAtendimento TipoAtendimento);
