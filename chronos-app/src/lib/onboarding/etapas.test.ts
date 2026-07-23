import { describe, expect, it } from "vitest"

import type { OrganizacaoAtual } from "@/api/organizacoes"
import { resolverEtapaOnboarding } from "@/lib/onboarding/etapas"

function organizacao(sobrescritas: Partial<OrganizacaoAtual> = {}): OrganizacaoAtual {
  return {
    organizacaoId: "org-1",
    nome: "Clínica Bem-Estar",
    enderecoPrestador: null,
    fusoHorario: null,
    possuiDisponibilidade: false,
    possuiServico: false,
    ...sobrescritas,
  }
}

describe("resolverEtapaOnboarding", () => {
  it("retorna organizacao quando não há organização", () => {
    expect(resolverEtapaOnboarding(null)).toBe("organizacao")
  })

  it("retorna fusoHorario quando a organização existe mas o fuso não foi configurado", () => {
    expect(resolverEtapaOnboarding(organizacao())).toBe("fusoHorario")
  })

  it("retorna disponibilidade quando o fuso está configurado mas não há disponibilidade", () => {
    expect(resolverEtapaOnboarding(organizacao({ fusoHorario: "America/Sao_Paulo" }))).toBe("disponibilidade")
  })

  it("retorna servico quando há disponibilidade mas não há serviço", () => {
    expect(
      resolverEtapaOnboarding(organizacao({ fusoHorario: "America/Sao_Paulo", possuiDisponibilidade: true })),
    ).toBe("servico")
  })

  it("retorna concluido quando fuso, disponibilidade e serviço já existem", () => {
    expect(
      resolverEtapaOnboarding(
        organizacao({ fusoHorario: "America/Sao_Paulo", possuiDisponibilidade: true, possuiServico: true }),
      ),
    ).toBe("concluido")
  })
})
