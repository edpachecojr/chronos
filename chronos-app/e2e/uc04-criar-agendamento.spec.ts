import { expect, test } from "@playwright/test"

import { diferencaEmDias, prepararCenarioComServicoEDisponibilidade, proximaSegundaIso } from "./support/cenario.js"
import { registrarNovoUsuarioEOnboard } from "./support/registro.js"

test("UC04 - cria um agendamento e ele aparece na agenda do dia", async ({ page }) => {
  await registrarNovoUsuarioEOnboard(page)
  await prepararCenarioComServicoEDisponibilidade(page)

  const dataIso = proximaSegundaIso()
  const dias = diferencaEmDias(dataIso)

  await page.goto("/")
  for (let i = 0; i < dias; i++) {
    await page.getByRole("button", { name: "Próximo dia" }).click()
  }

  await page.getByRole("button", { name: "Novo agendamento" }).click()
  await page.getByRole("combobox", { name: "Serviço" }).click()
  await page.getByRole("option", { name: "Consulta inicial" }).click()
  await page.getByLabel("Nome da pessoa atendida").fill("Maria Silva")
  await page.locator('input[name="data"]').fill(dataIso)
  await page.locator('input[name="hora"]').fill("10:00")
  await page.getByRole("button", { name: "Agendar" }).click()

  await expect(page.getByText("Consulta inicial · Maria Silva")).toBeVisible()
  await expect(page.getByText("10:00 – 10:50")).toBeVisible()
})
