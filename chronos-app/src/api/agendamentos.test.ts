import { describe, expect, it } from "vitest"

import { ErroApi } from "@/api/http"
import { paraInicioApi, traduzirErroDeAgendamento } from "@/api/agendamentos"

describe("paraInicioApi", () => {
  it("converte data+hora locais para um instante UTC com offset explícito", () => {
    const resultado = paraInicioApi("2026-07-27", "10:00")
    expect(resultado).toBe(new Date("2026-07-27T10:00:00").toISOString())
    expect(resultado.endsWith("Z")).toBe(true)
  })
})

describe("traduzirErroDeAgendamento", () => {
  it("traduz o 409 de perfil operacional não configurado com uma mensagem acionável", () => {
    const erro = new ErroApi("falha", 409, { codigo: "Agendamento.PerfilOperacionalNaoConfigurado" })
    expect(traduzirErroDeAgendamento(erro)).toBe(
      "A organização ainda não configurou endereço e fuso horário. Acesse Configurações para concluir.",
    )
  })

  it("usa uma mensagem genérica para código desconhecido", () => {
    const erro = new ErroApi("falha", 400, { codigo: "Agendamento.AlgoNuncaVisto" })
    expect(traduzirErroDeAgendamento(erro)).toBe("Não foi possível carregar a agenda. Tente novamente em instantes.")
  })

  it("usa uma mensagem genérica quando o erro não é um ErroApi", () => {
    expect(traduzirErroDeAgendamento(new Error("qualquer coisa"))).toBe(
      "Não foi possível carregar a agenda. Tente novamente em instantes.",
    )
  })
})
