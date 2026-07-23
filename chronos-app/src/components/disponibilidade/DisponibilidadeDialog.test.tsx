import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { beforeEach, describe, expect, it, vi } from "vitest"

import type { AuthContextValue } from "@/contexts/AuthContext"
import { AuthContext } from "@/contexts/AuthContext"
import { DisponibilidadeDialog } from "@/components/disponibilidade/DisponibilidadeDialog"

vi.mock("@/api/disponibilidades", async () => {
  const modulo = await vi.importActual<typeof import("@/api/disponibilidades")>("@/api/disponibilidades")
  return { ...modulo, criarDisponibilidade: vi.fn(), alterarDisponibilidade: vi.fn() }
})

const { criarDisponibilidade } = await import("@/api/disponibilidades")

function contextoAutenticado(): AuthContextValue {
  return {
    status: "autenticado_com_organizacao",
    organizacao: null,
    accessToken: "token-de-teste",
    profissionalId: "prof-1",
    entrar: vi.fn(),
    registrar: vi.fn(),
    completarOnboarding: vi.fn(),
    sair: vi.fn(),
  }
}

describe("DisponibilidadeDialog", () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it("cria uma nova disponibilidade com os dados preenchidos", async () => {
    vi.mocked(criarDisponibilidade).mockResolvedValue({ disponibilidadeId: "disp-1" })
    const onSalvo = vi.fn()
    const usuario = userEvent.setup()

    render(
      <AuthContext.Provider value={contextoAutenticado()}>
        <DisponibilidadeDialog profissionalId="prof-1" open onOpenChange={() => {}} onSalvo={onSalvo} />
      </AuthContext.Provider>,
    )

    await usuario.clear(screen.getByLabelText("Início"))
    await usuario.type(screen.getByLabelText("Início"), "09:00")
    await usuario.clear(screen.getByLabelText("Fim"))
    await usuario.type(screen.getByLabelText("Fim"), "18:00")

    await usuario.click(screen.getByRole("button", { name: "Salvar" }))

    await waitFor(() =>
      expect(criarDisponibilidade).toHaveBeenCalledWith(
        "prof-1",
        { diaDaSemana: "Monday", inicio: "09:00", fim: "18:00" },
        "token-de-teste",
      ),
    )
    expect(onSalvo).toHaveBeenCalled()
  })
})
