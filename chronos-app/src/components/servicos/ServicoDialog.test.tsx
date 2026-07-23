import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { beforeEach, describe, expect, it, vi } from "vitest"

import type { AuthContextValue } from "@/contexts/AuthContext"
import { AuthContext } from "@/contexts/AuthContext"
import { ServicoDialog } from "@/components/servicos/ServicoDialog"

vi.mock("@/api/servicos", async () => {
  const modulo = await vi.importActual<typeof import("@/api/servicos")>("@/api/servicos")
  return { ...modulo, criarServico: vi.fn(), atualizarServico: vi.fn() }
})

const { criarServico } = await import("@/api/servicos")

function contextoAutenticado(): AuthContextValue {
  return {
    status: "autenticado_onboarding_concluido",
    organizacao: null,
    accessToken: "token-de-teste",
    profissionalId: "prof-1",
    entrar: vi.fn(),
    registrar: vi.fn(),
    completarOnboarding: vi.fn(),
    refrescarOrganizacao: vi.fn(),
    sair: vi.fn(),
  }
}

describe("ServicoDialog", () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it("cria um novo serviço com os dados preenchidos", async () => {
    vi.mocked(criarServico).mockResolvedValue({ servicoId: "servico-1" })
    const onSalvo = vi.fn()
    const usuario = userEvent.setup()

    render(
      <AuthContext.Provider value={contextoAutenticado()}>
        <ServicoDialog profissionalId="prof-1" open onOpenChange={() => {}} onSalvo={onSalvo} />
      </AuthContext.Provider>,
    )

    await usuario.type(screen.getByLabelText("Nome do serviço"), "Consulta inicial")
    await usuario.clear(screen.getByLabelText("Duração (minutos)"))
    await usuario.type(screen.getByLabelText("Duração (minutos)"), "50")
    await usuario.clear(screen.getByLabelText("Preço (R$)"))
    await usuario.type(screen.getByLabelText("Preço (R$)"), "250")

    await usuario.click(screen.getByRole("button", { name: "Salvar" }))

    await waitFor(() => expect(criarServico).toHaveBeenCalledWith(
      "prof-1",
      { nome: "Consulta inicial", duracaoEmMinutos: 50, preco: 250, tipoAtendimento: "Online" },
      "token-de-teste",
    ))
    expect(onSalvo).toHaveBeenCalled()
  })
})
