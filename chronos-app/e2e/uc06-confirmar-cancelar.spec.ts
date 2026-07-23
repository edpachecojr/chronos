import { expect, test } from "@playwright/test"

import {
  criarAgendamentoViaApi,
  diferencaEmDias,
  prepararCenarioComServicoEDisponibilidade,
  proximaSegundaIso,
} from "./support/cenario.js"
import { registrarNovoUsuarioEOnboard } from "./support/registro.js"

test("UC06 - confirma um agendamento pendente e cancela em seguida", async ({ page }) => {
  await registrarNovoUsuarioEOnboard(page)
  const cenario = await prepararCenarioComServicoEDisponibilidade(page)
  const dataIso = proximaSegundaIso()
  const dias = diferencaEmDias(dataIso)

  await criarAgendamentoViaApi(page, cenario, dataIso)

  await page.goto("/")
  for (let i = 0; i < dias; i++) {
    await page.getByRole("button", { name: "Próximo dia" }).click()
  }

  await expect(page.getByText("Pendente", { exact: true })).toBeVisible()
  await page.getByRole("button", { name: "Confirmar agendamento de Maria Silva" }).click()
  await expect(page.getByText("Confirmado", { exact: true })).toBeVisible()

  await page.getByRole("button", { name: "Cancelar agendamento de Maria Silva" }).click()
  await page.getByRole("button", { name: "Cancelar agendamento" }).click()

  await expect(page.getByText("Nenhum agendamento neste dia")).toBeVisible()
})
