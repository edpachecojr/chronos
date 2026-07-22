using Chronos.Agenda.Domain.Compartilhado.EventosDominio;

namespace Chronos.Agenda.Domain.Organizacoes.EventosDominio;

/// <summary>Indica que uma organização passou a existir no domínio.</summary>
public sealed record OrganizacaoCriada(Guid OrganizacaoId, DateTime OcorridoEmUtc) : IEventoDominio;
