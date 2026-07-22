using Chronos.Agenda.Domain.Compartilhado.Resultados;

namespace Chronos.Agenda.Application.Agendamentos.Erros;

/// <summary>Catálogo de erros esperados no caso de uso de criação/reagendamento de agendamento.</summary>
public static class AgendamentoErros
{
    /// <summary>O nome informado para a pessoa atendida é inválido (comprimento fora do limite).</summary>
    public static Erro NomePessoaAtendidaInvalido(string mensagem) => new("Agendamento.NomePessoaAtendidaInvalido", mensagem);

    /// <summary>O serviço informado não é oferecido pelo profissional informado (RN04).</summary>
    public static Erro ServicoNaoPertenceAoProfissional(Guid servicoId, Guid profissionalId) => new(
        "Agendamento.ServicoNaoPertenceAoProfissional",
        $"O serviço {servicoId} não é oferecido pelo profissional {profissionalId}.");

    /// <summary>A organização não configurou endereço do prestador e fuso horário (perfil operacional), exigidos
    /// para calcular local ou disponibilidade do agendamento.</summary>
    public static Erro PerfilOperacionalNaoConfigurado(Guid organizacaoId) => new(
        "Agendamento.PerfilOperacionalNaoConfigurado",
        $"A organização {organizacaoId} ainda não configurou o perfil operacional (endereço do prestador e fuso horário).");

    /// <summary>O atendimento domiciliar exige o endereço da pessoa atendida, que não foi informado.</summary>
    public static readonly Erro EnderecoObrigatorioAusente = new(
        "Agendamento.EnderecoObrigatorioAusente",
        "O atendimento domiciliar exige o endereço da pessoa atendida.");

    /// <summary>O endereço informado é inválido (comprimento fora do limite).</summary>
    public static Erro EnderecoInvalido(string mensagem) => new("Agendamento.EnderecoInvalido", mensagem);

    /// <summary>O período calculado começa em um dia local e termina em outro; nenhuma janela de disponibilidade
    /// pode representar um intervalo que atravessa a meia-noite (ADR 0005).</summary>
    public static Erro PeriodoAtravessaMeiaNoite(DateTimeOffset inicioLocal, DateTimeOffset fimLocal) => new(
        "Agendamento.PeriodoAtravessaMeiaNoite",
        $"O período de {inicioLocal:O} a {fimLocal:O} atravessa a meia-noite no fuso horário da organização.");

    /// <summary>O período calculado se sobrepõe a outro agendamento ativo do mesmo profissional (RN02).</summary>
    public static Erro ConflitoDeAgenda(Guid profissionalId) => new(
        "Agendamento.ConflitoDeAgenda",
        $"O período informado se sobrepõe a outro agendamento ativo do profissional {profissionalId}.");
}
