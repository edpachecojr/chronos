import { ErroApi, requisitarApi } from "@/api/http"

export type RegistrarUsuarioEntrada = { email: string; password: string }

export type LoginEntrada = { email: string; password: string }

export type TokenAcesso = {
  tokenType: string
  accessToken: string
  expiresIn: number
  refreshToken: string
}

/** Cria a conta do usuário na Api (UC00). Não autentica — chame {@link autenticar} em seguida. */
export async function registrarUsuario(dados: RegistrarUsuarioEntrada): Promise<void> {
  await requisitarApi<void>("/v1/autenticacao/register", { method: "POST", body: dados })
}

export async function autenticar(dados: LoginEntrada): Promise<TokenAcesso> {
  return requisitarApi<TokenAcesso>("/v1/autenticacao/login", { method: "POST", body: dados })
}

const MENSAGENS_ERRO_CADASTRO: Record<string, string> = {
  DuplicateUserName: "Este e-mail já está cadastrado.",
  DuplicateEmail: "Este e-mail já está cadastrado.",
  InvalidEmail: "Informe um e-mail válido.",
  InvalidUserName: "Informe um e-mail válido.",
  PasswordTooShort: "A senha deve ter pelo menos 6 caracteres.",
  PasswordRequiresDigit: "A senha deve conter pelo menos um número.",
  PasswordRequiresLower: "A senha deve conter pelo menos uma letra minúscula.",
  PasswordRequiresUpper: "A senha deve conter pelo menos uma letra maiúscula.",
  PasswordRequiresNonAlphanumeric: "A senha deve conter pelo menos um caractere especial.",
}

/** Traduz a falha de {@link registrarUsuario} (ValidationProblemDetails nativo do Identity) para uma mensagem em pt-BR. */
export function traduzirErroDeCadastro(erro: unknown): string {
  if (!(erro instanceof ErroApi) || !erro.corpo || typeof erro.corpo !== "object") {
    return "Não foi possível concluir o cadastro. Tente novamente em instantes."
  }

  const errosPorCampo = (erro.corpo as { errors?: Record<string, string[]> }).errors ?? {}
  const codigos = Object.keys(errosPorCampo)
  const mensagens = codigos.map((codigo) => MENSAGENS_ERRO_CADASTRO[codigo]).filter(Boolean)

  return mensagens[0] ?? "Não foi possível concluir o cadastro. Verifique os dados informados."
}

/** Traduz a falha de {@link autenticar} para uma mensagem em pt-BR. */
export function traduzirErroDeLogin(erro: unknown): string {
  if (erro instanceof ErroApi && erro.status === 401) {
    return "E-mail ou senha inválidos."
  }
  return "Não foi possível entrar. Tente novamente em instantes."
}
