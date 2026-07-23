import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { beforeEach, describe, expect, it, vi } from "vitest"

import type { AuthContextValue } from "@/contexts/AuthContext"
import { AuthContext } from "@/contexts/AuthContext"
import { ConfiguracoesPage } from "@/pages/ConfiguracoesPage"

vi.mock("@/api/organizacoes", async () => {
  const modulo = await vi.importActual<typeof import("@/api/organizacoes")>("@/api/organizacoes")
  return { ...modulo, configurarPerfilOperacional: vi.fn() }
})

const { configurarPerfilOperacional } = await import("@/api/organizacoes")

function contexto(refrescarOrganizacao = vi.fn()): AuthContextValue {
  return {
    status: "autenticado_onboarding_concluido",
    organizacao: {
      organizacaoId: "org-1",
      nome: "Clínica Bem-Estar",
      enderecoPrestador: "Rua das Flores, 123",
      fusoHorario: "America/Manaus",
      possuiDisponibilidade: true,
      possuiServico: true,
    },
    accessToken: "token-de-teste",
    profissionalId: "prof-1",
    entrar: vi.fn(),
    registrar: vi.fn(),
    completarOnboarding: vi.fn(),
    refrescarOrganizacao,
    sair: vi.fn(),
  }
}

describe("ConfiguracoesPage", () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it("pré-preenche com o perfil operacional atual e salva as alterações", async () => {
    vi.mocked(configurarPerfilOperacional).mockResolvedValue(undefined)
    const refrescarOrganizacao = vi.fn()
    const usuario = userEvent.setup()

    render(
      <AuthContext.Provider value={contexto(refrescarOrganizacao)}>
        <ConfiguracoesPage />
      </AuthContext.Provider>,
    )

    expect(await screen.findByDisplayValue("Rua das Flores, 123")).toBeInTheDocument()

    await usuario.clear(screen.getByLabelText("Endereço do prestador (opcional)"))
    await usuario.type(screen.getByLabelText("Endereço do prestador (opcional)"), "Av. Nova, 45")
    await usuario.click(screen.getByRole("button", { name: "Salvar" }))

    await waitFor(() =>
      expect(configurarPerfilOperacional).toHaveBeenCalledWith(
        { fusoHorario: "America/Manaus", enderecoPrestador: "Av. Nova, 45" },
        "token-de-teste",
      ),
    )
    expect(refrescarOrganizacao).toHaveBeenCalled()
  })
})
