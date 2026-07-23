import { ErroApi, requisitarApi } from "@/api/http"

export type StatusAgendamento = "Pendente" | "Confirmado" | "Cancelado"

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

const MENSAGENS_ERRO_AGENDAMENTO: Record<string, string> = {
  "Agendamento.PerfilOperacionalNaoConfigurado":
    "A organização ainda não configurou endereço e fuso horário. Acesse Configurações para concluir.",
  "Profissional.NaoEncontrado": "Profissional não encontrado.",
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
