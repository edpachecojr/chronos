using Chronos.Agenda.Domain.Compartilhado.EventosDominio;

namespace Chronos.Agenda.Domain.Agendamentos.EventosDominio;

/// <summary>Indica que um novo agendamento pendente foi criado.</summary>
public sealed record AgendamentoCriado(Guid AgendamentoId, Guid OrganizacaoId, Guid ProfissionalId, Guid ServicoId, DateTime OcorridoEmUtc) : IEventoDominio;
