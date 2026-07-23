import { expect, test } from "@playwright/test"

import { diferencaEmDias, proximoDiaDaSemanaIso } from "./support/cenario.js"
import { completarWizardDeOnboarding, registrarNovoUsuario } from "./support/registro.js"

/**
 * Jornada completa do profissional pela UI, sem atalhos de Api: cria a conta, completa o onboarding (UC01) e usa
 * a disponibilidade/serviço criados nele para criar (UC04), visualizar (UC07), confirmar, editar (UC05) e cancelar
 * (UC06) um agendamento — cobrindo em sequência o mesmo caminho que um profissional percorre no primeiro uso real.
 */
test("UC08 - jornada completa: conta, onboarding, agendamento criado, confirmado, reagendado e cancelado", async ({
  page,
}) => {
  await registrarNovoUsuario(page)
  await completarWizardDeOnboarding(page)
  await page.waitForURL(/\/$/)
  await expect(page.getByRole("heading", { name: "Agenda" })).toBeVisible()

  // A disponibilidade criada no onboarding é domingo 06:00–07:00 (ver support/registro.ts).
  const dataIso = proximoDiaDaSemanaIso(0)
  const dias = diferencaEmDias(dataIso)
  for (let i = 0; i < dias; i++) {
    await page.getByRole("button", { name: "Próximo dia" }).click()
  }

  await page.getByRole("button", { name: "Novo agendamento" }).click()
  await page.getByRole("combobox", { name: "Serviço" }).click()
  await page.getByRole("option", { name: "Serviço E2E" }).click()
  await page.getByLabel("Nome da pessoa atendida").fill("Maria Silva")
  await page.locator('input[name="data"]').fill(dataIso)
  await page.locator('input[name="hora"]').fill("06:00")
  await page.getByRole("button", { name: "Agendar" }).click()

  // Criação (UC04) + visualização e detalhes do agendamento (UC07): serviço, pessoa, horário e status na mesma linha.
  await expect(page.getByText("Serviço E2E · Maria Silva")).toBeVisible()
  await expect(page.getByText("06:00 – 06:30")).toBeVisible()
  await expect(page.getByText("Pendente", { exact: true })).toBeVisible()

  await page.getByRole("button", { name: "Confirmar agendamento de Maria Silva" }).click()
  await expect(page.getByText("Confirmado", { exact: true })).toBeVisible()

  // Edição (UC05): reagenda para outro horário dentro da mesma janela de disponibilidade.
  await page.getByRole("button", { name: "Reagendar agendamento de Maria Silva" }).click()
  await page.locator('input[name="hora"]').fill("06:15")
  await page.getByRole("button", { name: "Salvar" }).click()
  await expect(page.getByText("06:15 – 06:45")).toBeVisible()

  // Exclusão (UC06): o domínio modela remoção como cancelamento — não há hard delete de agendamento.
  await page.getByRole("button", { name: "Cancelar agendamento de Maria Silva" }).click()
  await page.getByRole("button", { name: "Cancelar agendamento" }).click()
  await expect(page.getByText("Nenhum agendamento neste dia")).toBeVisible()
})
