using Chronos.Agenda.Domain.Compartilhado.Contratos;
using Chronos.Agenda.Domain.Compartilhado.Entidades;
using Chronos.Agenda.Domain.Compartilhado.ObjetosValor;
using Chronos.Agenda.Domain.Usuarios.Enums;
using Chronos.Agenda.Domain.Usuarios.EventosDominio;
using Chronos.Agenda.Domain.Usuarios.Exceptions;

namespace Chronos.Agenda.Domain.Usuarios.Entidades;

/// <summary>Representa quem acessa o Chronos e o papel que exerce em sua organização.</summary>
public sealed class Usuario : Entidade, IPertenceOrganizacao
{
    private Usuario(
        Guid id,
        Guid organizacaoId,
        Nome nome,
        PapelUsuario papel,
        Auditoria auditoria)
        : base(id, auditoria)
    {
        OrganizacaoId = organizacaoId;
        Nome = nome;
        Papel = papel;
    }

    public Guid OrganizacaoId { get; }
    public Nome Nome { get; private set; }
    public PapelUsuario Papel { get; }

    /// <summary>Cria um novo usuário vinculado a uma organização.</summary>
    /// <example><code>var usuario = Usuario.Criar(organizacaoId, nome, PapelUsuario.Proprietario, provedorDataHora);</code></example>
    public static Usuario Criar(Guid organizacaoId, Nome nome, PapelUsuario papel, IProvedorDataHora provedorDataHora)
    {
        ValidarOrganizacao(organizacaoId);
        var auditoria = Auditoria.Criar(provedorDataHora);
        var usuario = new Usuario(Guid.NewGuid(), organizacaoId, nome, papel, auditoria);
        usuario.LancarEventoDominio(new UsuarioCriado(usuario.Id, organizacaoId, papel, auditoria.CriadoEmUtc));
        return usuario;
    }

    public void Renomear(Nome nome, IProvedorDataHora provedorDataHora)
    {
        Nome = nome;
        Auditoria.Atualizar(provedorDataHora);
    }

    private static void ValidarOrganizacao(Guid organizacaoId)
    {
        if (organizacaoId == Guid.Empty)
        {
            throw new OrganizacaoUsuarioInvalidaException(organizacaoId);
        }
    }
}
