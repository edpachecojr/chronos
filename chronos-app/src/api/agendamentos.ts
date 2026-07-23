import { ErroApi, requisitarApi } from "@/api/http"

export type StatusAgendamento = "Pendente" | "Confirmado" | "Cancelado"

export type TipoPessoaAtendida = "Cliente" | "Paciente" | "Aluno" | "Outro"

export type JanelaHorario = { inicio: string; fim: string }

export type PeriodoOcupado = {
  agendamentoId: string
  inicio: string
  fim: string
  status: StatusAgendamento
  nomeServico: string
  nomePessoaAtendida: string
}

export type AgendaDiariaResultado = {
  data: string
  diaDaSemana: string
  janelasDisponiveis: JanelaHorario[]
  periodosOcupados: PeriodoOcupado[]
}

/** Consulta a agenda de um profissional em um único dia local (UC07). `data` no formato `"yyyy-MM-dd"`. */
export async function consultarAgendaDiaria(
  profissionalId: string,
  data: string,
  accessToken: string,
): Promise<AgendaDiariaResultado> {
  return requisitarApi<AgendaDiariaResultado>(`/v1/profissionais/${profissionalId}/agenda/diaria?data=${data}`, { accessToken })
}

export type AgendamentoFormulario = {
  servicoId: string
  nomePessoaAtendida: string
  tipoPessoaAtendida: TipoPessoaAtendida
  /** Formato `"yyyy-MM-dd"`, o que um `<input type="date">` produz. */
  data: string
  /** Formato `"HH:mm"`, o que um `<input type="time">` produz. */
  hora: string
  enderecoPessoaAtendida: string
}

/**
 * Combina data+hora do formulário (interpretados no fuso horário do navegador) em um instante UTC explícito, como
 * a Api exige (ADR 0005). Simplificação assumida no MVP: sem uma lib de fuso horário nas deps do frontend, o
 * fuso do navegador precisa coincidir com o `fusoHorario` configurado da organização para o horário ser
 * interpretado corretamente.
 */
export function paraInicioApi(data: string, hora: string): string {
  return new Date(`${data}T${hora}:00`).toISOString()
}

/** Cria um novo agendamento (UC04). */
export async function criarAgendamento(
  profissionalId: string,
  dados: AgendamentoFormulario,
  accessToken: string,
): Promise<{ agendamentoId: string }> {
  return requisitarApi("/v1/agendamentos/", {
    method: "POST",
    body: {
      profissionalId,
      servicoId: dados.servicoId,
      nomePessoaAtendida: dados.nomePessoaAtendida,
      tipoPessoaAtendida: dados.tipoPessoaAtendida,
      inicio: paraInicioApi(dados.data, dados.hora),
      enderecoPessoaAtendida: dados.enderecoPessoaAtendida || null,
    },
    accessToken,
  })
}

const MENSAGENS_ERRO_AGENDAMENTO: Record<string, string> = {
  "Agendamento.PerfilOperacionalNaoConfigurado":
    "A organização ainda não configurou endereço e fuso horário. Acesse Configurações para concluir.",
  "Profissional.NaoEncontrado": "Profissional não encontrado.",
  "Agendamento.LocalIncompativel": "O endereço informado não é compatível com a modalidade do serviço.",
  "Agendamento.NomePessoaAtendidaInvalido": "Informe um nome válido para a pessoa atendida.",
  "Agendamento.ServicoNaoPertenceAoProfissional": "Este serviço não pertence ao profissional selecionado.",
  "Agendamento.EnderecoObrigatorioAusente": "O atendimento domiciliar exige o endereço da pessoa atendida.",
  "Agendamento.EnderecoInvalido": "Informe um endereço válido.",
  "Agendamento.PeriodoAtravessaMeiaNoite": "O horário escolhido não pode atravessar a meia-noite.",
  "Agendamento.ConflitoDeAgenda": "Este horário já está ocupado por outro agendamento.",
  "Disponibilidade.ForaDaJanela": "Este horário está fora da disponibilidade configurada para o profissional.",
}

/** Traduz falhas dos casos de uso de agendamento (UC04-UC07) para uma mensagem em pt-BR. */
export function traduzirErroDeAgendamento(erro: unknown): string {
  if (erro instanceof ErroApi && erro.corpo && typeof erro.corpo === "object") {
    const codigo = (erro.corpo as { codigo?: string }).codigo
    if (codigo && MENSAGENS_ERRO_AGENDAMENTO[codigo]) {
      return MENSAGENS_ERRO_AGENDAMENTO[codigo]
    }
  }
  return "Não foi possível carregar a agenda. Tente novamente em instantes."
}
