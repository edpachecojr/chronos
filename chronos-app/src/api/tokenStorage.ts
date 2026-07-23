const CHAVE_ARMAZENAMENTO = "chronos:sessao"

export type SessaoArmazenada = {
  accessToken: string
  refreshToken: string
}

export function lerSessaoArmazenada(): SessaoArmazenada | null {
  const bruto = localStorage.getItem(CHAVE_ARMAZENAMENTO)
  if (!bruto) {
    return null
  }

  try {
    return JSON.parse(bruto) as SessaoArmazenada
  } catch {
    return null
  }
}

export function gravarSessaoArmazenada(sessao: SessaoArmazenada): void {
  localStorage.setItem(CHAVE_ARMAZENAMENTO, JSON.stringify(sessao))
}

export function limparSessaoArmazenada(): void {
  localStorage.removeItem(CHAVE_ARMAZENAMENTO)
}
