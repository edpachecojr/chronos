import { describe, expect, it } from "vitest"

import { ErroApi } from "@/api/http"
import { paraDuracaoApi, paraDuracaoEmMinutos, traduzirErroDeServico } from "@/api/servicos"

describe("paraDuracaoApi", () => {
  it("converte minutos para o formato HH:mm:ss esperado pela Api", () => {
    expect(paraDuracaoApi(50)).toBe("00:50:00")
    expect(paraDuracaoApi(90)).toBe("01:30:00")
    expect(paraDuracaoApi(5)).toBe("00:05:00")
  })
})

describe("paraDuracaoEmMinutos", () => {
  it("converte o HH:mm:ss retornado pela Api de volta para minutos", () => {
    expect(paraDuracaoEmMinutos("00:50:00")).toBe(50)
    expect(paraDuracaoEmMinutos("01:30:00")).toBe(90)
  })
})

describe("traduzirErroDeServico", () => {
  it("traduz um código de erro conhecido", () => {
    const erro = new ErroApi("falha", 400, { codigo: "Servico.PrecoInvalido" })
    expect(traduzirErroDeServico(erro)).toBe("Informe um preço válido, com até duas casas decimais.")
  })

  it("usa uma mensagem genérica para código desconhecido", () => {
    const erro = new ErroApi("falha", 400, { codigo: "Servico.AlgoNuncaVisto" })
    expect(traduzirErroDeServico(erro)).toBe("Não foi possível salvar o serviço. Tente novamente em instantes.")
  })

  it("usa uma mensagem genérica quando o erro não é um ErroApi", () => {
    expect(traduzirErroDeServico(new Error("qualquer coisa"))).toBe(
      "Não foi possível salvar o serviço. Tente novamente em instantes.",
    )
  })
})
