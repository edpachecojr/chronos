import { describe, expect, it } from "vitest"

import { criarEsquemaAgendamento } from "@/lib/validation/agendamentoSchemas"

const DADOS_VALIDOS = {
  servicoId: "servico-online",
  nomePessoaAtendida: "Maria Silva",
  tipoPessoaAtendida: "Paciente" as const,
  data: "2026-07-27",
  hora: "10:00",
  enderecoPessoaAtendida: "",
}

const TIPO_ATENDIMENTO_POR_SERVICO = { "servico-online": "Online" as const, "servico-domiciliar": "Domiciliar" as const }

describe("criarEsquemaAgendamento", () => {
  it("aceita dados válidos para um serviço online sem endereço", () => {
    const esquema = criarEsquemaAgendamento(TIPO_ATENDIMENTO_POR_SERVICO)
    expect(esquema.safeParse(DADOS_VALIDOS).success).toBe(true)
  })

  it("rejeita quando o serviço é domiciliar e o endereço está vazio", () => {
    const esquema = criarEsquemaAgendamento(TIPO_ATENDIMENTO_POR_SERVICO)
    const resultado = esquema.safeParse({ ...DADOS_VALIDOS, servicoId: "servico-domiciliar" })
    expect(resultado.success).toBe(false)
  })

  it("aceita quando o serviço é domiciliar e o endereço foi informado", () => {
    const esquema = criarEsquemaAgendamento(TIPO_ATENDIMENTO_POR_SERVICO)
    const resultado = esquema.safeParse({
      ...DADOS_VALIDOS,
      servicoId: "servico-domiciliar",
      enderecoPessoaAtendida: "Rua das Flores, 123",
    })
    expect(resultado.success).toBe(true)
  })

  it("rejeita nome da pessoa atendida vazio", () => {
    const esquema = criarEsquemaAgendamento(TIPO_ATENDIMENTO_POR_SERVICO)
    const resultado = esquema.safeParse({ ...DADOS_VALIDOS, nomePessoaAtendida: "" })
    expect(resultado.success).toBe(false)
  })
})
