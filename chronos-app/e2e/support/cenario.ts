import type { Page } from "@playwright/test"

import { lerAccessToken } from "./registro.js"

const API_BASE = "http://localhost:5001"

export type CenarioComAgenda = {
  profissionalId: string
  servicoId: string
  accessToken: string
}

/**
 * Prepara, via chamadas diretas à Api (mais rápido e focado do que repetir fluxos de UI já cobertos pelos specs
 * de UC02/UC03), o que UC04/UC05/UC06/UC07 precisam para criar ou consultar agendamentos: perfil operacional
 * configurado, um serviço e uma disponibilidade cobrindo o horário usado nos cenários.
 */
export async function prepararCenarioComServicoEDisponibilidade(page: Page): Promise<CenarioComAgenda> {
  const accessToken = await lerAccessToken(page)
  const headers = { Authorization: `Bearer ${accessToken}`, "Content-Type": "application/json" }

  const respostaProfissionais = await page.request.get(`${API_BASE}/v1/profissionais/`, { headers })
  const [profissional] = (await respostaProfissionais.json()) as { profissionalId: string }[]
  const profissionalId = profissional.profissionalId

  await page.request.put(`${API_BASE}/v1/organizacoes/perfil-operacional`, {
    headers,
    data: { enderecoPrestador: null, fusoHorario: "America/Sao_Paulo" },
  })

  const respostaServico = await page.request.post(`${API_BASE}/v1/servicos/`, {
    headers,
    data: { profissionalId, nome: "Consulta inicial", duracao: "00:50:00", preco: 250, tipoAtendimento: "Online" },
  })
  const { servicoId } = (await respostaServico.json()) as { servicoId: string }

  await page.request.post(`${API_BASE}/v1/disponibilidades/`, {
    headers,
    data: { profissionalId, diaDaSemana: "Monday", inicio: "09:00:00", fim: "18:00:00" },
  })

  return { profissionalId, servicoId, accessToken }
}

/** Cria um agendamento diretamente via Api, para specs que testam uma ação subsequente (reagendar, confirmar, cancelar). */
export async function criarAgendamentoViaApi(
  page: Page,
  cenario: CenarioComAgenda,
  dataIso: string,
  hora = "10:00:00",
): Promise<{ agendamentoId: string }> {
  const resposta = await page.request.post(`${API_BASE}/v1/agendamentos/`, {
    headers: { Authorization: `Bearer ${cenario.accessToken}`, "Content-Type": "application/json" },
    data: {
      profissionalId: cenario.profissionalId,
      servicoId: cenario.servicoId,
      nomePessoaAtendida: "Maria Silva",
      tipoPessoaAtendida: "Paciente",
      inicio: `${dataIso}T${hora}-03:00`,
      enderecoPessoaAtendida: null,
    },
  })
  return (await resposta.json()) as { agendamentoId: string }
}

/** Próxima ocorrência (sempre futura, nunca hoje) do `diaDaSemana` informado (0 = domingo .. 6 = sábado), em `"yyyy-MM-dd"`. */
export function proximoDiaDaSemanaIso(diaDaSemana: number): string {
  const hoje = new Date()
  const diferenca = (diaDaSemana - hoje.getDay() + 7) % 7 || 7
  const data = new Date(hoje)
  data.setDate(hoje.getDate() + diferenca)
  return data.toISOString().slice(0, 10)
}

/** Próxima segunda-feira a partir de hoje, em `"yyyy-MM-dd"` — mesmo dia da semana da disponibilidade fixa do cenário. */
export function proximaSegundaIso(): string {
  return proximoDiaDaSemanaIso(1)
}

/** Quantidade de dias entre hoje e a data informada, para navegar a agenda com o botão "Próximo dia". */
export function diferencaEmDias(dataIso: string): number {
  const hoje = new Date()
  const inicioDeHoje = new Date(hoje.getFullYear(), hoje.getMonth(), hoje.getDate())
  const alvo = new Date(`${dataIso}T00:00:00`)
  return Math.round((alvo.getTime() - inicioDeHoje.getTime()) / (1000 * 60 * 60 * 24))
}
