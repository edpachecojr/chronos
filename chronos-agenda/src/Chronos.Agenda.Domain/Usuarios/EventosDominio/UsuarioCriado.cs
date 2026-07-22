using Chronos.Agenda.Domain.Compartilhado.EventosDominio;
using Chronos.Agenda.Domain.Usuarios.Enums;

namespace Chronos.Agenda.Domain.Usuarios.EventosDominio;

/// <summary>Indica que um usuário foi criado e vinculado a uma organização.</summary>
public sealed record UsuarioCriado(Guid UsuarioId, Guid OrganizacaoId, PapelUsuario Papel, DateTime OcorridoEmUtc) : IEventoDominio;
