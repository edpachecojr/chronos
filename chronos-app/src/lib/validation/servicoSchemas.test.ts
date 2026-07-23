import { describe, expect, it } from "vitest"

import { esquemaServico } from "@/lib/validation/servicoSchemas"

const DADOS_VALIDOS = { nome: "Consulta inicial", duracaoEmMinutos: 50, preco: 250, tipoAtendimento: "Online" as const }

describe("esquemaServico", () => {
  it("aceita dados válidos", () => {
    expect(esquemaServico.safeParse(DADOS_VALIDOS).success).toBe(true)
  })

  it("rejeita nome vazio", () => {
    const resultado = esquemaServico.safeParse({ ...DADOS_VALIDOS, nome: "" })
    expect(resultado.success).toBe(false)
  })

  it("rejeita duração maior que 12 horas", () => {
    const resultado = esquemaServico.safeParse({ ...DADOS_VALIDOS, duracaoEmMinutos: 721 })
    expect(resultado.success).toBe(false)
  })

  it("rejeita duração zero ou negativa", () => {
    expect(esquemaServico.safeParse({ ...DADOS_VALIDOS, duracaoEmMinutos: 0 }).success).toBe(false)
    expect(esquemaServico.safeParse({ ...DADOS_VALIDOS, duracaoEmMinutos: -10 }).success).toBe(false)
  })

  it("rejeita preço negativo", () => {
    const resultado = esquemaServico.safeParse({ ...DADOS_VALIDOS, preco: -1 })
    expect(resultado.success).toBe(false)
  })

  it("rejeita preço com mais de duas casas decimais", () => {
    const resultado = esquemaServico.safeParse({ ...DADOS_VALIDOS, preco: 10.999 })
    expect(resultado.success).toBe(false)
  })

  it("rejeita modalidade inválida", () => {
    const resultado = esquemaServico.safeParse({ ...DADOS_VALIDOS, tipoAtendimento: "Presencial" })
    expect(resultado.success).toBe(false)
  })
})
