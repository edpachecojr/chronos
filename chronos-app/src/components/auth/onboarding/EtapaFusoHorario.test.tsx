import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { beforeEach, describe, expect, it, vi } from "vitest"

import { EtapaFusoHorario } from "@/components/auth/onboarding/EtapaFusoHorario"
import type { AuthContextValue } from "@/contexts/AuthContext"
import { AuthContext } from "@/contexts/AuthContext"

vi.mock("@/api/organizacoes", async () => {
  const modulo = await vi.importActual<typeof import("@/api/organizacoes")>("@/api/organizacoes")
  return { ...modulo, configurarPerfilOperacional: vi.fn() }
})

const { configurarPerfilOperacional } = await import("@/api/organizacoes")

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

describe("EtapaFusoHorario", () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it("pré-preenche o fuso horário com o padrão UTC-3 e salva sem endereço", async () => {
    vi.mocked(configurarPerfilOperacional).mockResolvedValue(undefined)
    const refrescarOrganizacao = vi.fn()
    const usuario = userEvent.setup()

    render(
      <AuthContext.Provider value={contextoAutenticado(refrescarOrganizacao)}>
        <EtapaFusoHorario />
      </AuthContext.Provider>,
    )

    expect(screen.getByRole("combobox", { name: "Fuso horário" })).toHaveTextContent("Horário de Brasília (UTC-3)")

    await usuario.click(screen.getByRole("button", { name: "Continuar" }))

    await waitFor(() =>
      expect(configurarPerfilOperacional).toHaveBeenCalledWith(
        { fusoHorario: "America/Sao_Paulo", enderecoPrestador: null },
        "token-de-teste",
      ),
    )
    expect(refrescarOrganizacao).toHaveBeenCalled()
  })

  it("envia o endereço do prestador quando preenchido", async () => {
    vi.mocked(configurarPerfilOperacional).mockResolvedValue(undefined)
    const usuario = userEvent.setup()

    render(
      <AuthContext.Provider value={contextoAutenticado()}>
        <EtapaFusoHorario />
      </AuthContext.Provider>,
    )

    await usuario.type(screen.getByLabelText("Endereço do prestador (opcional)"), "Av. Central, 20")
    await usuario.click(screen.getByRole("button", { name: "Continuar" }))

    await waitFor(() =>
      expect(configurarPerfilOperacional).toHaveBeenCalledWith(
        { fusoHorario: "America/Sao_Paulo", enderecoPrestador: "Av. Central, 20" },
        "token-de-teste",
      ),
    )
  })
})
