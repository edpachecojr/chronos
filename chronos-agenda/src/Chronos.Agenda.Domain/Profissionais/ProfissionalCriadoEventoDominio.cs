using Chronos.Agenda.Domain.Compartilhado;

namespace Chronos.Agenda.Domain.Profissionais;

/// <summary>Indica que um profissional foi criado em uma organização.</summary>
public sealed record ProfissionalCriadoEventoDominio(Guid ProfissionalId, Guid OrganizacaoId, DateTime OcorridoEmUtc) : IEventoDominio;
