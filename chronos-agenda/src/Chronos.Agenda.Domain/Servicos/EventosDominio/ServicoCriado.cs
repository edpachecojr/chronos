using Chronos.Agenda.Domain.Compartilhado.EventosDominio;

namespace Chronos.Agenda.Domain.Servicos.EventosDominio;

/// <summary>Indica que um serviço passou a ser oferecido por um profissional.</summary>
public sealed record ServicoCriado(Guid ServicoId, Guid OrganizacaoId, Guid ProfissionalId, DateTime OcorridoEmUtc) : IEventoDominio;
