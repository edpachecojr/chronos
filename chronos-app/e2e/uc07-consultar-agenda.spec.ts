import { expect, test } from "@playwright/test"

import {
  criarAgendamentoViaApi,
  diferencaEmDias,
  prepararCenarioComServicoEDisponibilidade,
  proximaSegundaIso,
} from "./support/cenario.js"
import { registrarNovoUsuarioEOnboard } from "./support/registro.js"

test("UC07 - consulta a agenda do dia e vê nome do serviço e da pessoa atendida", async ({ page }) => {
  await registrarNovoUsuarioEOnboard(page)
  const cenario = await prepararCenarioComServicoEDisponibilidade(page)
  const dataIso = proximaSegundaIso()
  const dias = diferencaEmDias(dataIso)

  await criarAgendamentoViaApi(page, cenario, dataIso)

  await page.goto("/")
  for (let i = 0; i < dias; i++) {
    await page.getByRole("button", { name: "Próximo dia" }).click()
  }

  await expect(page.getByText("09:00 – 18:00")).toBeVisible()
  await expect(page.getByText("Consulta inicial · Maria Silva")).toBeVisible()
})
