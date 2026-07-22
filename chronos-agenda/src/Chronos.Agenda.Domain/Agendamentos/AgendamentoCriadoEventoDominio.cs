using Chronos.Agenda.Domain.Compartilhado;

namespace Chronos.Agenda.Domain.Agendamentos;

/// <summary>Indica que um novo agendamento pendente foi criado.</summary>
public sealed record AgendamentoCriadoEventoDominio(Guid AgendamentoId, Guid OrganizacaoId, Guid ProfissionalId, Guid ServicoId, DateTime OcorridoEmUtc) : IEventoDominio;
