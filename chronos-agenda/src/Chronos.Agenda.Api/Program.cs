using Chronos.Agenda.Api.Extensions;
using Chronos.Agenda.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AdicionarPersistencia(builder.Configuration)
    .AdicionarIdentity()
    .AdicionarRepositorios()
    .AdicionarServicoAutenticacao()
    .AdicionarContextoUsuario();

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UsarContextoUsuario();
app.UseAuthorization();

// Os endpoints da Api (incluindo os de Identity via MapIdentityApi) ainda não foram implementados.

app.Run();
