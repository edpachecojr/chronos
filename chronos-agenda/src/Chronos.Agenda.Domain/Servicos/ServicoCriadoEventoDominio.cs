using Chronos.Agenda.Domain.Compartilhado;

namespace Chronos.Agenda.Domain.Servicos;

/// <summary>Indica que um serviço passou a ser oferecido por um profissional.</summary>
public sealed record ServicoCriadoEventoDominio(Guid ServicoId, Guid OrganizacaoId, Guid ProfissionalId, DateTime OcorridoEmUtc) : IEventoDominio;
