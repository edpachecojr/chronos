import { ErroApi, requisitarApi } from "@/api/http"

export type TipoAtendimento = "Online" | "Domiciliar" | "NoEnderecoDoPrestador"

export type ServicoResumo = {
  servicoId: string
  nome: string
  /** Formato `"HH:mm:ss"`, espelhando o `TimeSpan` da Api — use {@link paraDuracaoEmMinutos} para exibição. */
  duracao: string
  preco: number
  tipoAtendimento: TipoAtendimento
}

export type ServicoFormulario = {
  nome: string
  duracaoEmMinutos: number
  preco: number
  tipoAtendimento: TipoAtendimento
}

/** Converte minutos (o que o formulário coleta) para o formato `"HH:mm:ss"` que a Api espera para `TimeSpan`. */
export function paraDuracaoApi(duracaoEmMinutos: number): string {
  const horas = Math.floor(duracaoEmMinutos / 60)
  const minutosRestantes = duracaoEmMinutos % 60
  return `${String(horas).padStart(2, "0")}:${String(minutosRestantes).padStart(2, "0")}:00`
}

/** Converte o `"HH:mm:ss"` retornado pela Api de volta para minutos, para exibição no formulário/lista. */
export function paraDuracaoEmMinutos(duracao: string): number {
  const [horas, minutos] = duracao.split(":").map(Number)
  return horas * 60 + minutos
}

/** Lista os serviços do catálogo de um profissional (UC03). */
export async function listarServicos(profissionalId: string, accessToken: string): Promise<ServicoResumo[]> {
  return requisitarApi<ServicoResumo[]>(`/v1/servicos/?profissionalId=${profissionalId}`, { accessToken })
}

/** Cria um novo serviço no catálogo de um profissional (UC03). */
export async function criarServico(
  profissionalId: string,
  dados: ServicoFormulario,
  accessToken: string,
): Promise<{ servicoId: string }> {
  return requisitarApi("/v1/servicos/", {
    method: "POST",
    body: {
      profissionalId,
      nome: dados.nome,
      duracao: paraDuracaoApi(dados.duracaoEmMinutos),
      preco: dados.preco,
      tipoAtendimento: dados.tipoAtendimento,
    },
    accessToken,
  })
}

/** Atualiza a configuração comercial de um serviço existente (UC03). */
export async function atualizarServico(servicoId: string, dados: ServicoFormulario, accessToken: string): Promise<void> {
  await requisitarApi<void>(`/v1/servicos/${servicoId}`, {
    method: "PUT",
    body: {
      nome: dados.nome,
      duracao: paraDuracaoApi(dados.duracaoEmMinutos),
      preco: dados.preco,
      tipoAtendimento: dados.tipoAtendimento,
    },
    accessToken,
  })
}

const MENSAGENS_ERRO_SERVICO: Record<string, string> = {
  "Servico.NomeInvalido": "Informe um nome de serviço válido (até 120 caracteres).",
  "Servico.DuracaoInvalida": "Informe uma duração válida (até 12 horas).",
  "Servico.PrecoInvalido": "Informe um preço válido, com até duas casas decimais.",
  "Servico.NaoEncontrado": "Este serviço não foi encontrado.",
  "Profissional.NaoEncontrado": "Profissional não encontrado.",
}

/** Traduz a falha de {@link criarServico}/{@link atualizarServico} para uma mensagem em pt-BR. */
export function traduzirErroDeServico(erro: unknown): string {
  if (erro instanceof ErroApi && erro.corpo && typeof erro.corpo === "object") {
    const codigo = (erro.corpo as { codigo?: string }).codigo
    if (codigo && MENSAGENS_ERRO_SERVICO[codigo]) {
      return MENSAGENS_ERRO_SERVICO[codigo]
    }
  }
  return "Não foi possível salvar o serviço. Tente novamente em instantes."
}
