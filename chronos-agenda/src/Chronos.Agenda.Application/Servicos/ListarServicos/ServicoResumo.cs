using Chronos.Agenda.Domain.Servicos.Enums;

namespace Chronos.Agenda.Application.Servicos.ListarServicos;

/// <summary>Projeção somente leitura de um serviço no catálogo de um profissional (UC03).</summary>
public sealed record ServicoResumo(Guid ServicoId, string Nome, TimeSpan Duracao, decimal Preco, TipoAtendimento TipoAtendimento);
