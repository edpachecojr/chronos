import { renderHook, waitFor } from "@testing-library/react"
import { beforeEach, describe, expect, it, vi } from "vitest"

import type { AuthContextValue } from "@/contexts/AuthContext"
import { AuthContext } from "@/contexts/AuthContext"
import { useProfissionalAtual } from "@/hooks/useProfissionalAtual"

vi.mock("@/api/profissionais", () => ({
  listarProfissionais: vi.fn(),
}))

const { listarProfissionais } = await import("@/api/profissionais")

function contextoComToken(accessToken: string | null): AuthContextValue {
  return {
    status: "autenticado_onboarding_concluido",
    organizacao: null,
    accessToken,
    profissionalId: null,
    entrar: vi.fn(),
    registrar: vi.fn(),
    completarOnboarding: vi.fn(),
    refrescarOrganizacao: vi.fn(),
    sair: vi.fn(),
  }
}

describe("useProfissionalAtual", () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it("resolve o profissional único da organização", async () => {
    vi.mocked(listarProfissionais).mockResolvedValue([{ profissionalId: "prof-1", nome: "Ana Souza" }])

    const { result } = renderHook(() => useProfissionalAtual(), {
      wrapper: ({ children }) => <AuthContext.Provider value={contextoComToken("token")}>{children}</AuthContext.Provider>,
    })

    expect(result.current.carregando).toBe(true)
    await waitFor(() => expect(result.current.carregando).toBe(false))
    expect(result.current.profissionalId).toBe("prof-1")
    expect(result.current.erro).toBeNull()
  })

  it("sem access token, não carrega e não chama a Api", () => {
    const { result } = renderHook(() => useProfissionalAtual(), {
      wrapper: ({ children }) => <AuthContext.Provider value={contextoComToken(null)}>{children}</AuthContext.Provider>,
    })

    expect(result.current.carregando).toBe(false)
    expect(result.current.profissionalId).toBeNull()
    expect(listarProfissionais).not.toHaveBeenCalled()
  })

  it("quando a organização não tem nenhum profissional, expõe um erro", async () => {
    vi.mocked(listarProfissionais).mockResolvedValue([])

    const { result } = renderHook(() => useProfissionalAtual(), {
      wrapper: ({ children }) => <AuthContext.Provider value={contextoComToken("token")}>{children}</AuthContext.Provider>,
    })

    await waitFor(() => expect(result.current.carregando).toBe(false))
    expect(result.current.profissionalId).toBeNull()
    expect(result.current.erro).not.toBeNull()
  })
})
