import type { Page } from "@playwright/test"

export type UsuarioRegistrado = {
  email: string
}

/**
 * Registra um usuário com e-mail único, completa o login e o onboarding (UC01) pela UI, deixando a página no
 * dashboard ("/"). Cada cenário chama isto para começar com uma organização/profissional isolados.
 */
export async function registrarNovoUsuarioEOnboard(page: Page): Promise<UsuarioRegistrado> {
  const email = `e2e+${Date.now()}-${Math.random().toString(36).slice(2)}@chronos.test`
  const senha = "Senha123!"

  await page.goto("/registro")
  await page.getByLabel("E-mail").fill(email)
  await page.getByLabel("Senha", { exact: true }).fill(senha)
  await page.getByLabel("Confirmar senha").fill(senha)
  await page.getByLabel(/aceito os termos/i).check()
  await page.getByRole("button", { name: "Criar conta" }).click()

  await page.waitForURL(/\/onboarding$/)
  await page.getByLabel("Nome do seu negócio").fill("Organização E2E")
  await page.getByLabel("Seu nome").fill("Profissional E2E")
  await page.getByRole("button", { name: "Concluir" }).click()

  await page.waitForURL(/\/$/)

  return { email }
}

/** Lê o access token persistido pelo AuthProvider, para chamadas diretas à Api fora da UI (ex.: setup de cenário). */
export async function lerAccessToken(page: Page): Promise<string> {
  const sessao = await page.evaluate(() => localStorage.getItem("chronos:sessao"))
  if (!sessao) {
    throw new Error("Nenhuma sessão encontrada no localStorage — o usuário está autenticado?")
  }
  return (JSON.parse(sessao) as { accessToken: string }).accessToken
}
