import { expect, test } from "@playwright/test"

import { registrarNovoUsuarioEOnboard } from "./support/registro.js"

test("UC02 - configura uma disponibilidade semanal e ela aparece na lista", async ({ page }) => {
  await registrarNovoUsuarioEOnboard(page)

  await page.goto("/disponibilidade")
  await page.getByRole("button", { name: "Nova disponibilidade" }).first().click()
  await page.locator('input[name="inicio"]').fill("09:00")
  await page.locator('input[name="fim"]').fill("18:00")
  await page.getByRole("button", { name: "Salvar" }).click()
  await expect(page.getByRole("dialog")).toBeHidden()

  await expect(page.getByRole("cell", { name: "Segunda-feira", exact: true })).toBeVisible()
  await expect(page.getByRole("cell", { name: "09:00", exact: true })).toBeVisible()
  await expect(page.getByRole("cell", { name: "18:00", exact: true })).toBeVisible()
})
