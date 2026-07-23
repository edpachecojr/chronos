import type { Page } from "@playwright/test"

export type UsuarioRegistrado = {
  email: string
}

/** Registra um usuário com e-mail único pela UI, deixando a página em `/onboarding` (etapa 1, organização). */
export async function registrarNovoUsuario(page: Page): Promise<UsuarioRegistrado> {
  const email = `e2e+${Date.now()}-${Math.random().toString(36).slice(2)}@chronos.test`
  const senha = "Senha123!"

  await page.goto("/registro")
  await page.getByLabel("E-mail").fill(email)
  await page.getByLabel("Senha", { exact: true }).fill(senha)
  await page.getByLabel("Confirmar senha").fill(senha)
  await page.getByLabel(/aceito os termos/i).check()
  await page.getByRole("button", { name: "Criar conta" }).click()

  await page.waitForURL(/\/onboarding$/)

  return { email }
}

/**
 * Registra um usuário com e-mail único, completa o login e as 4 etapas do onboarding (UC01 — organização, fuso
 * horário, disponibilidade e primeiro serviço) pela UI, deixando a página no dashboard ("/"). Cada cenário chama
 * isto para começar com uma organização/profissional isolados.
 *
 * A disponibilidade adicionada aqui (domingo, 06:00–07:00) é só para satisfazer o requisito mínimo do onboarding
 * (RN07); o horário foi escolhido para não colidir com nenhum `HH:mm` usado pelos specs de UC02–UC07 (a maioria
 * usa segunda-feira, 09:00–18:00, tanto pela UI quanto por `prepararCenarioComServicoEDisponibilidade`).
 */
export async function registrarNovoUsuarioEOnboard(page: Page): Promise<UsuarioRegistrado> {
  const usuario = await registrarNovoUsuario(page)
  await completarWizardDeOnboarding(page)
  await page.waitForURL(/\/$/)

  return usuario
}

/** Preenche as 4 etapas do wizard de onboarding, assumindo que a página já está em `/onboarding` na etapa 1. */
export async function completarWizardDeOnboarding(page: Page): Promise<void> {
  await page.getByLabel("Nome do seu negócio").fill("Organização E2E")
  await page.getByLabel("Seu nome").fill("Profissional E2E")
  await page.getByRole("button", { name: "Continuar" }).click()

  await page.getByText("Etapa 2 de 4").waitFor()
  await page.getByRole("button", { name: "Continuar" }).click()

  await page.getByText("Etapa 3 de 4").waitFor()
  await page.getByRole("combobox", { name: "Dia da semana" }).click()
  await page.getByRole("option", { name: "Domingo" }).click()
  await page.locator('input[name="inicio"]').fill("06:00")
  await page.locator('input[name="fim"]').fill("07:00")
  await page.getByRole("button", { name: "Adicionar disponibilidade" }).click()
  await page.getByRole("button", { name: "Continuar" }).click()

  await page.getByText("Etapa 4 de 4").waitFor()
  await page.getByLabel("Nome do serviço").fill("Serviço E2E")
  await page.locator('input[name="duracaoEmMinutos"]').fill("30")
  await page.locator('input[name="preco"]').fill("100")
  await page.getByRole("button", { name: "Concluir" }).click()
}

/** Lê o access token persistido pelo AuthProvider, para chamadas diretas à Api fora da UI (ex.: setup de cenário). */
export async function lerAccessToken(page: Page): Promise<string> {
  const sessao = await page.evaluate(() => localStorage.getItem("chronos:sessao"))
  if (!sessao) {
    throw new Error("Nenhuma sessão encontrada no localStorage — o usuário está autenticado?")
  }
  return (JSON.parse(sessao) as { accessToken: string }).accessToken
}
