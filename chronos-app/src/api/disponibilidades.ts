import { ErroApi, requisitarApi } from "@/api/http"

export type DiaDaSemana = "Sunday" | "Monday" | "Tuesday" | "Wednesday" | "Thursday" | "Friday" | "Saturday"

export const ROTULOS_DIA_DA_SEMANA: Record<DiaDaSemana, string> = {
  Sunday: "Domingo",
  Monday: "Segunda-feira",
  Tuesday: "Terça-feira",
  Wednesday: "Quarta-feira",
  Thursday: "Quinta-feira",
  Friday: "Sexta-feira",
  Saturday: "Sábado",
}

export type DisponibilidadeResumo = {
  disponibilidadeId: string
  diaDaSemana: DiaDaSemana
  /** Formato `"HH:mm:ss"`, espelhando o `TimeOnly` da Api. */
  inicio: string
  fim: string
}

export type DisponibilidadeFormulario = {
  diaDaSemana: DiaDaSemana
  /** Formato `"HH:mm"`, o que um `<input type="time">` produz. */
  inicio: string
  fim: string
}

function paraHorarioApi(horario: string): string {
  return horario.length === 5 ? `${horario}:00` : horario
}

/** Lista as disponibilidades semanais configuradas de um profissional (UC02). */
export async function listarDisponibilidades(profissionalId: string, accessToken: string): Promise<DisponibilidadeResumo[]> {
  return requisitarApi<DisponibilidadeResumo[]>(`/v1/disponibilidades/?profissionalId=${profissionalId}`, { accessToken })
}

/** Cria uma nova janela de disponibilidade semanal para um profissional (UC02). */
export async function criarDisponibilidade(
  profissionalId: string,
  dados: DisponibilidadeFormulario,
  accessToken: string,
): Promise<{ disponibilidadeId: string }> {
  return requisitarApi("/v1/disponibilidades/", {
    method: "POST",
    body: {
      profissionalId,
      diaDaSemana: dados.diaDaSemana,
      inicio: paraHorarioApi(dados.inicio),
      fim: paraHorarioApi(dados.fim),
    },
    accessToken,
  })
}

/** Altera o dia ou a janela de uma disponibilidade semanal existente (UC02). */
export async function alterarDisponibilidade(
  disponibilidadeId: string,
  profissionalId: string,
  dados: DisponibilidadeFormulario,
  accessToken: string,
): Promise<void> {
  await requisitarApi<void>(`/v1/disponibilidades/${disponibilidadeId}`, {
    method: "PUT",
    body: {
      profissionalId,
      diaDaSemana: dados.diaDaSemana,
      inicio: paraHorarioApi(dados.inicio),
      fim: paraHorarioApi(dados.fim),
    },
    accessToken,
  })
}

/** Remove uma janela de disponibilidade semanal existente (UC02). `profissionalId` vai na query string, como a Api exige. */
export async function removerDisponibilidade(
  disponibilidadeId: string,
  profissionalId: string,
  accessToken: string,
): Promise<void> {
  await requisitarApi<void>(`/v1/disponibilidades/${disponibilidadeId}?profissionalId=${profissionalId}`, {
    method: "DELETE",
    accessToken,
  })
}

const MENSAGENS_ERRO_DISPONIBILIDADE: Record<string, string> = {
  "Disponibilidade.JanelaInvalida": "Informe uma janela válida (o fim deve ser depois do início).",
  "Disponibilidade.JanelaSobreposta": "Esta janela se sobrepõe a outra disponibilidade já configurada para este dia.",
  "Disponibilidade.NaoEncontrada": "Esta disponibilidade não foi encontrada.",
  "Profissional.NaoEncontrado": "Profissional não encontrado.",
}

/** Traduz a falha de {@link criarDisponibilidade}/{@link alterarDisponibilidade}/{@link removerDisponibilidade} para uma mensagem em pt-BR. */
export function traduzirErroDeDisponibilidade(erro: unknown): string {
  if (erro instanceof ErroApi && erro.corpo && typeof erro.corpo === "object") {
    const codigo = (erro.corpo as { codigo?: string }).codigo
    if (codigo && MENSAGENS_ERRO_DISPONIBILIDADE[codigo]) {
      return MENSAGENS_ERRO_DISPONIBILIDADE[codigo]
    }
  }
  return "Não foi possível salvar a disponibilidade. Tente novamente em instantes."
}
