import { ErroApi, requisitarApi } from "@/api/http"

export type OnboardOrganizacaoEntrada = { nome: string; nomeProfissionalInicial: string }

export type OnboardOrganizacaoResultado = { organizacaoId: string; profissionalId: string }

export type OrganizacaoAtual = {
  organizacaoId: string
  nome: string
  enderecoPrestador: string | null
  fusoHorario: string | null
  possuiDisponibilidade: boolean
  possuiServico: boolean
}

export type PerfilOperacionalEntrada = { enderecoPrestador: string | null; fusoHorario: string }

const MENSAGENS_ERRO_ONBOARDING: Record<string, string> = {
  "Organizacao.NomeInvalido": "Informe um nome de negócio válido (até 120 caracteres).",
  "Profissional.NomeInvalido": "Informe um nome de profissional válido.",
}

const MENSAGENS_ERRO_PERFIL_OPERACIONAL: Record<string, string> = {
  "Organizacao.EnderecoInvalido": "Informe um endereço válido (até 300 caracteres).",
  "Organizacao.FusoHorarioInvalido": "Selecione um fuso horário válido.",
  "Organizacao.NaoEncontrada": "Organização não encontrada.",
}

/** Cria a organização e o primeiro profissional do usuário autenticado (UC01). */
export async function onboardOrganizacao(
  dados: OnboardOrganizacaoEntrada,
  accessToken: string,
): Promise<OnboardOrganizacaoResultado> {
  return requisitarApi<OnboardOrganizacaoResultado>("/v1/organizacoes/", {
    method: "POST",
    body: dados,
    accessToken,
  })
}

/** Resolve a organização do usuário autenticado, ou `null` se o onboarding (UC01) ainda não foi concluído. */
export async function consultarOrganizacaoAtual(accessToken: string): Promise<OrganizacaoAtual | null> {
  const resultado = await requisitarApi<OrganizacaoAtual | null>("/v1/organizacoes/atual", { accessToken })
  return resultado ?? null
}

/** Traduz a falha de {@link onboardOrganizacao} (ProblemDetails do Result Pattern) para uma mensagem em pt-BR. */
export function traduzirErroDeOnboarding(erro: unknown): string {
  if (erro instanceof ErroApi && erro.corpo && typeof erro.corpo === "object") {
    const codigo = (erro.corpo as { codigo?: string }).codigo
    if (codigo && MENSAGENS_ERRO_ONBOARDING[codigo]) {
      return MENSAGENS_ERRO_ONBOARDING[codigo]
    }
  }
  return "Não foi possível concluir o onboarding. Tente novamente em instantes."
}

/** Configura o endereço do prestador (opcional) e o fuso horário da organização corrente (UC04, UC05, UC07). */
export async function configurarPerfilOperacional(dados: PerfilOperacionalEntrada, accessToken: string): Promise<void> {
  await requisitarApi<void>("/v1/organizacoes/perfil-operacional", {
    method: "PUT",
    body: dados,
    accessToken,
  })
}

/** Traduz a falha de {@link configurarPerfilOperacional} (ProblemDetails do Result Pattern) para uma mensagem em pt-BR. */
export function traduzirErroDePerfilOperacional(erro: unknown): string {
  if (erro instanceof ErroApi && erro.corpo && typeof erro.corpo === "object") {
    const codigo = (erro.corpo as { codigo?: string }).codigo
    if (codigo && MENSAGENS_ERRO_PERFIL_OPERACIONAL[codigo]) {
      return MENSAGENS_ERRO_PERFIL_OPERACIONAL[codigo]
    }
  }
  return "Não foi possível salvar o perfil operacional. Tente novamente em instantes."
}
