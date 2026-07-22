using Chronos.Agenda.Domain.Compartilhado;

namespace Chronos.Agenda.Domain.Organizacoes;

/// <summary>Indica que uma organização passou a existir no domínio.</summary>
public sealed record OrganizacaoCriadaEventoDominio(Guid OrganizacaoId, DateTime OcorridoEmUtc) : IEventoDominio;
