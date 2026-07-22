namespace Chronos.Agenda.Domain.Compartilhado.Contratos;

/// <summary>
/// Expõe o identificador do usuário autenticado (Identity) da sessão corrente
/// e a organização à qual ele pertence. A resolução — autenticação, consulta
/// ao vínculo usuário/organização — é responsabilidade das camadas de
/// Aplicação/Infraestrutura; o domínio apenas declara o contrato para que
/// casos de uso recebam a organização corrente sem se acoplar ao mecanismo de
/// autenticação (ver ADR 0003).
/// </summary>
public interface IContextoUsuario
{
    Guid UsuarioId { get; }

    /// <summary>Resolve a organização vinculada ao usuário da sessão corrente.</summary>
    /// <example><code>var organizacaoId = contextoUsuario.ObterOrganizacaoId();</code></example>
    Guid ObterOrganizacaoId();
}
