using Chronos.Agenda.Domain.Compartilhado.EventosDominio;

namespace Chronos.Agenda.Domain.Profissionais.EventosDominio;

/// <summary>Indica que um profissional foi criado em uma organização.</summary>
public sealed record ProfissionalCriado(Guid ProfissionalId, Guid OrganizacaoId, DateTime OcorridoEmUtc) : IEventoDominio;
