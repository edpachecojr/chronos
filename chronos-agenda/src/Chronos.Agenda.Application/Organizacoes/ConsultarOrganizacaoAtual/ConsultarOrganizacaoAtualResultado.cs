namespace Chronos.Agenda.Application.Organizacoes.ConsultarOrganizacaoAtual;

/// <summary>Identificação, perfil operacional e progresso de onboarding da organização vinculada ao usuário
/// autenticado. <see cref="EnderecoPrestador"/>/<see cref="FusoHorario"/> são nulos até o perfil operacional ser
/// configurado (ver <c>ConfigurarPerfilOperacional</c>, UC04). <see cref="PossuiDisponibilidade"/> e
/// <see cref="PossuiServico"/> são derivados a cada consulta (não é um campo de progresso persistido): refletem se
/// o profissional já tem, respectivamente, alguma disponibilidade semanal e algum serviço cadastrados, os dados
/// mínimos exigidos para criar um agendamento. O frontend usa os quatro campos para decidir em qual etapa do
/// onboarding retomar.</summary>
public sealed record ConsultarOrganizacaoAtualResultado(
    Guid OrganizacaoId,
    string Nome,
    string? EnderecoPrestador,
    string? FusoHorario,
    bool PossuiDisponibilidade,
    bool PossuiServico);
