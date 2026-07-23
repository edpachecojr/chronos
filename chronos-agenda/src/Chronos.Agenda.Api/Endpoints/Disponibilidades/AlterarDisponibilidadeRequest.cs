namespace Chronos.Agenda.Api.Endpoints.Disponibilidades;

/// <summary>Corpo da requisição para alterar o dia ou a janela de uma disponibilidade semanal existente (UC02). O
/// identificador da disponibilidade vem da rota, não do corpo.</summary>
public sealed record AlterarDisponibilidadeRequest(Guid ProfissionalId, DayOfWeek DiaDaSemana, TimeOnly Inicio, TimeOnly Fim);
