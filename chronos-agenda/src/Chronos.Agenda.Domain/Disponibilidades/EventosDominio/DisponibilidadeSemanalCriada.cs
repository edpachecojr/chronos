using Chronos.Agenda.Domain.Compartilhado.EventosDominio;

namespace Chronos.Agenda.Domain.Disponibilidades.EventosDominio;

/// <summary>Indica que uma disponibilidade semanal foi configurada para um profissional.</summary>
public sealed record DisponibilidadeSemanalCriada(Guid DisponibilidadeId, Guid OrganizacaoId, Guid ProfissionalId, DateTime OcorridoEmUtc) : IEventoDominio;
