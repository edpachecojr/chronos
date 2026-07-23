import { expect, test } from "@playwright/test"

import { completarWizardDeOnboarding, registrarNovoUsuario } from "./support/registro.js"

test("UC01 - completa as 4 etapas do onboarding e chega na agenda", async ({ page }) => {
  await registrarNovoUsuario(page)
  await completarWizardDeOnboarding(page)

  await page.waitForURL(/\/$/)
  await expect(page.getByRole("heading", { name: "Agenda" })).toBeVisible()
})

test("UC01 - retoma o onboarding na etapa correta após recarregar a página", async ({ page }) => {
  await registrarNovoUsuario(page)

  await page.getByLabel("Nome do seu negócio").fill("Organização E2E")
  await page.getByLabel("Seu nome").fill("Profissional E2E")
  await page.getByRole("button", { name: "Continuar" }).click()
  await page.getByText("Etapa 2 de 4").waitFor()

  await page.reload()

  await expect(page.getByText("Etapa 2 de 4")).toBeVisible()
  await expect(page.getByLabel("Nome do seu negócio")).not.toBeVisible()
})
