import { describe, expect, it } from "vitest"

import { esquemaDisponibilidade } from "@/lib/validation/disponibilidadeSchemas"

const DADOS_VALIDOS = { diaDaSemana: "Monday" as const, inicio: "09:00", fim: "18:00" }

describe("esquemaDisponibilidade", () => {
  it("aceita dados válidos", () => {
    expect(esquemaDisponibilidade.safeParse(DADOS_VALIDOS).success).toBe(true)
  })

  it("rejeita fim antes do início", () => {
    const resultado = esquemaDisponibilidade.safeParse({ ...DADOS_VALIDOS, inicio: "18:00", fim: "09:00" })
    expect(resultado.success).toBe(false)
  })

  it("rejeita fim igual ao início", () => {
    const resultado = esquemaDisponibilidade.safeParse({ ...DADOS_VALIDOS, inicio: "09:00", fim: "09:00" })
    expect(resultado.success).toBe(false)
  })

  it("rejeita dia da semana inválido", () => {
    const resultado = esquemaDisponibilidade.safeParse({ ...DADOS_VALIDOS, diaDaSemana: "Someday" })
    expect(resultado.success).toBe(false)
  })
})
