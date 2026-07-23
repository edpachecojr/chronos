import { describe, expect, it } from "vitest"

import { ErroApi } from "@/api/http"
import { traduzirErroDeDisponibilidade } from "@/api/disponibilidades"

describe("traduzirErroDeDisponibilidade", () => {
  it("traduz um código de erro conhecido", () => {
    const erro = new ErroApi("falha", 409, { codigo: "Disponibilidade.JanelaSobreposta" })
    expect(traduzirErroDeDisponibilidade(erro)).toBe(
      "Esta janela se sobrepõe a outra disponibilidade já configurada para este dia.",
    )
  })

  it("usa uma mensagem genérica para código desconhecido", () => {
    const erro = new ErroApi("falha", 400, { codigo: "Disponibilidade.AlgoNuncaVisto" })
    expect(traduzirErroDeDisponibilidade(erro)).toBe("Não foi possível salvar a disponibilidade. Tente novamente em instantes.")
  })

  it("usa uma mensagem genérica quando o erro não é um ErroApi", () => {
    expect(traduzirErroDeDisponibilidade(new Error("qualquer coisa"))).toBe(
      "Não foi possível salvar a disponibilidade. Tente novamente em instantes.",
    )
  })
})
