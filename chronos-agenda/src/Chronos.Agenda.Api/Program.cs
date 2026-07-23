using System.Text.Json.Serialization;
using Chronos.Agenda.Api.Endpoints;
using Chronos.Agenda.Api.ExceptionHandling;
using Chronos.Agenda.Api.Extensions;
using Chronos.Agenda.Application.Extensions;
using Chronos.Agenda.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AdicionarPersistencia(builder.Configuration)
    .AdicionarIdentity()
    .AdicionarRepositorios()
    .AdicionarServicoAutenticacao()
    .AdicionarContextoUsuario()
    .AdicionarCasosDeUso();

builder.Services.AddAuthorization();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<TratadorGlobalDeExcecoes>();
builder.Services.ConfigureHttpJsonOptions(opcoes => opcoes.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

app.UseExceptionHandler();
app.UseAuthentication();
app.UsarContextoUsuario();
app.UseAuthorization();
app.MapearEndpoints();

app.Run();
