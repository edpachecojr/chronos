import { expect, test } from "@playwright/test"

import { criarAgendamentoViaApi, diferencaEmDias, prepararCenarioComServicoEDisponibilidade, proximaSegundaIso } from "./support/cenario.js"
import { registrarNovoUsuario, registrarNovoUsuarioEOnboard } from "./support/registro.js"

test("cadastro com e-mail já usado exibe erro e mantém o usuário na tela de registro", async ({ page }) => {
  const usuario = await registrarNovoUsuario(page)

  // registrarNovoUsuario já autentica (onboarding pendente); limpa a sessão para acessar /registro como visitante,
  // já que PublicOnlyRoute afasta quem está autenticado dessa rota.
  await page.evaluate(() => localStorage.removeItem("chronos:sessao"))
  await page.goto("/registro")
  await page.getByLabel("E-mail").fill(usuario.email)
  await page.getByLabel("Senha", { exact: true }).fill("Senha123!")
  await page.getByLabel("Confirmar senha").fill("Senha123!")
  await page.getByLabel(/aceito os termos/i).check()
  await page.getByRole("button", { name: "Criar conta" }).click()

  await expect(page.getByRole("alert")).toContainText("Este e-mail já está cadastrado.")
  await expect(page).toHaveURL(/\/registro$/)
})

test("login com senha incorreta exibe erro", async ({ page }) => {
  const usuario = await registrarNovoUsuarioEOnboard(page)

  await page.getByRole("button", { name: "Abrir menu do perfil" }).click()
  await page.getByRole("menuitem", { name: "Sair" }).click()
  await page.waitForURL(/\/login$/)

  await page.getByLabel("E-mail").fill(usuario.email)
  await page.getByLabel("Senha", { exact: true }).fill("SenhaErrada123!")
  await page.getByRole("button", { name: "Entrar" }).click()

  await expect(page.getByRole("alert")).toContainText("E-mail ou senha inválidos.")
  await expect(page).toHaveURL(/\/login$/)
})

test("login com credenciais corretas após logout retorna à agenda", async ({ page }) => {
  const usuario = await registrarNovoUsuarioEOnboard(page)

  await page.getByRole("button", { name: "Abrir menu do perfil" }).click()
  await page.getByRole("menuitem", { name: "Sair" }).click()
  await page.waitForURL(/\/login$/)

  await page.getByLabel("E-mail").fill(usuario.email)
  await page.getByLabel("Senha", { exact: true }).fill("Senha123!")
  await page.getByRole("button", { name: "Entrar" }).click()

  await page.waitForURL(/\/$/)
  await expect(page.getByRole("heading", { name: "Agenda" })).toBeVisible()
})

test("criar agendamento em horário já ocupado exibe erro de conflito e não duplica o agendamento", async ({ page }) => {
  await registrarNovoUsuarioEOnboard(page)
  const cenario = await prepararCenarioComServicoEDisponibilidade(page)
  const dataIso = proximaSegundaIso()
  const dias = diferencaEmDias(dataIso)

  await criarAgendamentoViaApi(page, cenario, dataIso, "10:00:00")

  await page.goto("/")
  for (let i = 0; i < dias; i++) {
    await page.getByRole("button", { name: "Próximo dia" }).click()
  }

  await page.getByRole("button", { name: "Novo agendamento" }).click()
  await page.getByRole("combobox", { name: "Serviço" }).click()
  await page.getByRole("option", { name: "Consulta inicial" }).click()
  await page.getByLabel("Nome da pessoa atendida").fill("João Souza")
  await page.locator('input[name="data"]').fill(dataIso)
  await page.locator('input[name="hora"]').fill("10:00")
  await page.getByRole("button", { name: "Agendar" }).click()

  await expect(page.getByRole("alert")).toContainText("Este horário já está ocupado por outro agendamento.")
  await page.getByRole("dialog").getByRole("button", { name: "Cancelar" }).click()

  await expect(page.getByText("João Souza")).not.toBeVisible()
  await expect(page.getByText("Consulta inicial · Maria Silva")).toBeVisible()
})
