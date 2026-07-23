import { requisitarApi } from "@/api/http"

export type ProfissionalResumo = { profissionalId: string; nome: string }

/** Lista os profissionais vinculados à organização corrente do usuário autenticado. */
export async function listarProfissionais(accessToken: string): Promise<ProfissionalResumo[]> {
  return requisitarApi<ProfissionalResumo[]>("/v1/profissionais/", { accessToken })
}
