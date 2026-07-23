import { expect, test } from "@playwright/test"

import { registrarNovoUsuarioEOnboard } from "./support/registro.js"

test("UC03 - cria e edita um serviço do catálogo", async ({ page }) => {
  await registrarNovoUsuarioEOnboard(page)

  await page.goto("/servicos")
  await page.getByRole("button", { name: "Novo serviço" }).first().click()
  await page.getByLabel("Nome do serviço").fill("Consulta inicial")
  await page.locator('input[name="duracaoEmMinutos"]').fill("50")
  await page.locator('input[name="preco"]').fill("250")
  await page.getByRole("button", { name: "Salvar" }).click()
  await expect(page.getByRole("dialog")).toBeHidden()

  await expect(page.getByRole("cell", { name: "Consulta inicial", exact: true })).toBeVisible()
  await expect(page.getByRole("cell", { name: "R$ 250,00" })).toBeVisible()

  await page.getByRole("button", { name: "Editar Consulta inicial" }).click()
  await page.getByLabel("Nome do serviço").fill("Consulta de retorno")
  await page.locator('input[name="preco"]').fill("180")
  await page.getByRole("button", { name: "Salvar" }).click()
  await expect(page.getByRole("dialog")).toBeHidden()

  await expect(page.getByRole("cell", { name: "Consulta de retorno", exact: true })).toBeVisible()
  await expect(page.getByRole("cell", { name: "R$ 180,00" })).toBeVisible()
})
