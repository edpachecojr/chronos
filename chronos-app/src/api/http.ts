const URL_BASE_API = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5080"

export class ErroApi extends Error {
  readonly status: number
  readonly corpo: unknown

  constructor(message: string, status: number, corpo: unknown) {
    super(message)
    this.name = "ErroApi"
    this.status = status
    this.corpo = corpo
  }
}

type RequisicaoOpcoes = {
  method?: "GET" | "POST" | "PUT" | "DELETE"
  body?: unknown
  accessToken?: string
}

/** Executa uma requisição à Api do Chronos e decodifica a resposta como JSON, lançando {@link ErroApi} em respostas de erro. */
export async function requisitarApi<T>(caminho: string, opcoes: RequisicaoOpcoes = {}): Promise<T> {
  const resposta = await fetch(`${URL_BASE_API}${caminho}`, {
    method: opcoes.method ?? "GET",
    headers: montarCabecalhos(opcoes.accessToken),
    body: opcoes.body ? JSON.stringify(opcoes.body) : undefined,
  })

  const corpo = await lerCorpoJson(resposta)
  if (!resposta.ok) {
    throw new ErroApi(`Falha na requisição a ${caminho} (status ${resposta.status})`, resposta.status, corpo)
  }

  return corpo as T
}

function montarCabecalhos(accessToken?: string): HeadersInit {
  const cabecalhos: Record<string, string> = { "Content-Type": "application/json" }
  if (accessToken) {
    cabecalhos.Authorization = `Bearer ${accessToken}`
  }
  return cabecalhos
}

async function lerCorpoJson(resposta: Response): Promise<unknown> {
  const texto = await resposta.text()
  if (!texto) {
    return null
  }
  try {
    return JSON.parse(texto)
  } catch {
    return null
  }
}
