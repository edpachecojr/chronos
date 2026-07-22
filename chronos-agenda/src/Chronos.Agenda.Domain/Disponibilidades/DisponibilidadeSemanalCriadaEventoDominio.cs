using Chronos.Agenda.Domain.Compartilhado;

namespace Chronos.Agenda.Domain.Disponibilidades;

/// <summary>Indica que uma disponibilidade semanal foi configurada para um profissional.</summary>
public sealed record DisponibilidadeSemanalCriadaEventoDominio(Guid DisponibilidadeId, Guid OrganizacaoId, Guid ProfissionalId, DateTime OcorridoEmUtc) : IEventoDominio;
