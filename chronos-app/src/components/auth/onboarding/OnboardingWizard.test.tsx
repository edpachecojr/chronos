import { render, screen } from "@testing-library/react"
import { describe, expect, it, vi } from "vitest"

import type { OrganizacaoAtual } from "@/api/organizacoes"
import { OnboardingWizard } from "@/components/auth/onboarding/OnboardingWizard"
import type { AuthContextValue } from "@/contexts/AuthContext"
import { AuthContext } from "@/contexts/AuthContext"

vi.mock("@/components/auth/OnboardingForm", () => ({
  OnboardingForm: () => <div>etapa-organizacao</div>,
}))
vi.mock("@/components/auth/onboarding/EtapaFusoHorario", () => ({
  EtapaFusoHorario: () => <div>etapa-fuso-horario</div>,
}))
vi.mock("@/components/auth/onboarding/EtapaDisponibilidade", () => ({
  EtapaDisponibilidade: () => <div>etapa-disponibilidade</div>,
}))
vi.mock("@/components/auth/onboarding/EtapaServico", () => ({
  EtapaServico: () => <div>etapa-servico</div>,
}))

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

function contextoCom(organizacaoAtual: OrganizacaoAtual | null): AuthContextValue {
  return {
    status: "autenticado_onboarding_pendente",
    organizacao: organizacaoAtual,
    accessToken: "token-de-teste",
    profissionalId: null,
    entrar: vi.fn(),
    registrar: vi.fn(),
    completarOnboarding: vi.fn(),
    refrescarOrganizacao: vi.fn(),
    sair: vi.fn(),
  }
}

function renderComContexto(organizacaoAtual: OrganizacaoAtual | null) {
  return render(
    <AuthContext.Provider value={contextoCom(organizacaoAtual)}>
      <OnboardingWizard />
    </AuthContext.Provider>,
  )
}

describe("OnboardingWizard", () => {
  it("mostra a etapa de organização quando ainda não há organização", () => {
    renderComContexto(null)
    expect(screen.getByText("etapa-organizacao")).toBeInTheDocument()
  })

  it("mostra a etapa de fuso horário quando a organização existe sem fuso configurado", () => {
    renderComContexto(organizacao())
    expect(screen.getByText("etapa-fuso-horario")).toBeInTheDocument()
    expect(screen.getByText(/Etapa 2 de 4/)).toBeInTheDocument()
  })

  it("mostra a etapa de disponibilidade quando o fuso já está configurado", () => {
    renderComContexto(organizacao({ fusoHorario: "America/Sao_Paulo" }))
    expect(screen.getByText("etapa-disponibilidade")).toBeInTheDocument()
    expect(screen.getByText(/Etapa 3 de 4/)).toBeInTheDocument()
  })

  it("mostra a etapa de serviço quando já há disponibilidade", () => {
    renderComContexto(organizacao({ fusoHorario: "America/Sao_Paulo", possuiDisponibilidade: true }))
    expect(screen.getByText("etapa-servico")).toBeInTheDocument()
    expect(screen.getByText(/Etapa 4 de 4/)).toBeInTheDocument()
  })

  it("não renderiza nenhuma etapa quando o onboarding já está concluído", () => {
    const { container } = renderComContexto(
      organizacao({ fusoHorario: "America/Sao_Paulo", possuiDisponibilidade: true, possuiServico: true }),
    )
    expect(container).toBeEmptyDOMElement()
  })
})
