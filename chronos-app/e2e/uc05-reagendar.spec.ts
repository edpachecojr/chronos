import { expect, test } from "@playwright/test"

import {
  criarAgendamentoViaApi,
  diferencaEmDias,
  prepararCenarioComServicoEDisponibilidade,
  proximaSegundaIso,
} from "./support/cenario.js"
import { registrarNovoUsuarioEOnboard } from "./support/registro.js"

test("UC05 - reagenda um agendamento existente para outro horário", async ({ page }) => {
  await registrarNovoUsuarioEOnboard(page)
  const cenario = await prepararCenarioComServicoEDisponibilidade(page)
  const dataIso = proximaSegundaIso()
  const dias = diferencaEmDias(dataIso)

  await criarAgendamentoViaApi(page, cenario, dataIso)

  await page.goto("/")
  for (let i = 0; i < dias; i++) {
    await page.getByRole("button", { name: "Próximo dia" }).click()
  }

  await expect(page.getByText("10:00 – 10:50")).toBeVisible()
  await page.getByRole("button", { name: "Reagendar agendamento de Maria Silva" }).click()
  await page.locator('input[name="hora"]').fill("11:00")
  await page.getByRole("button", { name: "Salvar" }).click()

  await expect(page.getByText("11:00 – 11:50")).toBeVisible()
})
