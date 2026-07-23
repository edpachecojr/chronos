import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { beforeEach, describe, expect, it, vi } from "vitest"

import { EtapaDisponibilidade } from "@/components/auth/onboarding/EtapaDisponibilidade"
import type { AuthContextValue } from "@/contexts/AuthContext"
import { AuthContext } from "@/contexts/AuthContext"

vi.mock("@/api/profissionais", () => ({
  listarProfissionais: vi.fn(),
}))
vi.mock("@/api/disponibilidades", async () => {
  const modulo = await vi.importActual<typeof import("@/api/disponibilidades")>("@/api/disponibilidades")
  return { ...modulo, criarDisponibilidade: vi.fn(), listarDisponibilidades: vi.fn() }
})

const { listarProfissionais } = await import("@/api/profissionais")
const { criarDisponibilidade, listarDisponibilidades } = await import("@/api/disponibilidades")

function contextoAutenticado(refrescarOrganizacao = vi.fn()): AuthContextValue {
  return {
    status: "autenticado_onboarding_pendente",
    organizacao: null,
    accessToken: "token-de-teste",
    profissionalId: null,
    entrar: vi.fn(),
    registrar: vi.fn(),
    completarOnboarding: vi.fn(),
    refrescarOrganizacao,
    sair: vi.fn(),
  }
}

describe("EtapaDisponibilidade", () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.mocked(listarProfissionais).mockResolvedValue([{ profissionalId: "prof-1", nome: "Ana Souza" }])
  })

  it("mantém Continuar desabilitado até haver ao menos uma disponibilidade salva", async () => {
    vi.mocked(listarDisponibilidades).mockResolvedValue([])
    vi.mocked(criarDisponibilidade).mockResolvedValue({ disponibilidadeId: "disp-1" })
    const refrescarOrganizacao = vi.fn()
    const usuario = userEvent.setup()

    render(
      <AuthContext.Provider value={contextoAutenticado(refrescarOrganizacao)}>
        <EtapaDisponibilidade />
      </AuthContext.Provider>,
    )

    await waitFor(() => expect(listarDisponibilidades).toHaveBeenCalledWith("prof-1", "token-de-teste"))
    expect(screen.getByRole("button", { name: "Continuar" })).toBeDisabled()

    vi.mocked(listarDisponibilidades).mockResolvedValue([
      { disponibilidadeId: "disp-1", diaDaSemana: "Monday", inicio: "09:00:00", fim: "18:00:00" },
    ])

    await usuario.click(screen.getByRole("button", { name: "Adicionar disponibilidade" }))

    await waitFor(() =>
      expect(criarDisponibilidade).toHaveBeenCalledWith(
        "prof-1",
        { diaDaSemana: "Monday", inicio: "09:00", fim: "18:00" },
        "token-de-teste",
      ),
    )
    await waitFor(() => expect(screen.getByRole("button", { name: "Continuar" })).toBeEnabled())

    await usuario.click(screen.getByRole("button", { name: "Continuar" }))
    expect(refrescarOrganizacao).toHaveBeenCalled()
  })
})
