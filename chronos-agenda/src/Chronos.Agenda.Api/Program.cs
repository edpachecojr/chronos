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
    .AdicionarCasosDeUso()
    .AdicionarCorsFrontend(builder.Configuration)
    .AdicionarSwagger();

builder.Services.AddAuthorization();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<TratadorGlobalDeExcecoes>();
builder.Services.ConfigureHttpJsonOptions(opcoes => opcoes.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors(Chronos.Agenda.Api.Extensions.ServiceCollectionExtensions.PoliticaCorsFrontend);
app.UseAuthentication();
app.UsarContextoUsuario();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
    app.UsarSwagger();

app.MapearEndpoints();

app.Run();
