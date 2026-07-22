using Chronos.Agenda.Application.Compartilhado.Contratos;
using Chronos.Agenda.Application.Compartilhado.Erros;
using Chronos.Agenda.Domain.Compartilhado.Resultados;
using Microsoft.AspNetCore.Identity;

namespace Chronos.Agenda.Infrastructure.Identity;

/// <summary>Implementação de <see cref="IServicoAutenticacao"/> sobre o ASP.NET Core Identity (ADR 0001). O
/// <see cref="CancellationToken"/> recebido não é repassado ao <see cref="UserManager{TUser}"/>/
/// <see cref="SignInManager{TUser}"/>: essas APIs do Identity não expõem esse parâmetro em seus métodos públicos.</summary>
public sealed class ServicoAutenticacao(
    UserManager<UsuarioIdentity> gerenciadorDeUsuarios,
    SignInManager<UsuarioIdentity> gerenciadorDeAutenticacao) : IServicoAutenticacao
{
    public async Task<Resultado<Guid>> CriarUsuarioAsync(string email, string senha, CancellationToken cancellationToken)
    {
        var usuario = new UsuarioIdentity
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
        };

        var resultado = await gerenciadorDeUsuarios.CreateAsync(usuario, senha);
        if (resultado.Succeeded)
        {
            return Resultado<Guid>.Ok(usuario.Id);
        }

        if (resultado.Errors.Any(erro => erro.Code is "DuplicateUserName" or "DuplicateEmail"))
        {
            return Resultado<Guid>.Falha(AutenticacaoErros.EmailJaCadastrado(email));
        }

        return Resultado<Guid>.Falha(AutenticacaoErros.CriacaoInvalida(resultado.Errors.Select(erro => erro.Description)));
    }

    public async Task<Resultado<Guid>> AutenticarAsync(string email, string senha, CancellationToken cancellationToken)
    {
        var usuario = await gerenciadorDeUsuarios.FindByEmailAsync(email);
        if (usuario is null)
        {
            return Resultado<Guid>.Falha(AutenticacaoErros.CredenciaisInvalidas);
        }

        var resultadoAutenticacao = await gerenciadorDeAutenticacao.CheckPasswordSignInAsync(usuario, senha, lockoutOnFailure: true);
        if (!resultadoAutenticacao.Succeeded)
        {
            return Resultado<Guid>.Falha(AutenticacaoErros.CredenciaisInvalidas);
        }

        return Resultado<Guid>.Ok(usuario.Id);
    }

    public async Task<Guid?> BuscarPorEmailAsync(string email, CancellationToken cancellationToken)
    {
        var usuario = await gerenciadorDeUsuarios.FindByEmailAsync(email);
        return usuario?.Id;
    }
}
