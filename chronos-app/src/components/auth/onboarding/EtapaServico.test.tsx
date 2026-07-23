import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { beforeEach, describe, expect, it, vi } from "vitest"

import { EtapaServico } from "@/components/auth/onboarding/EtapaServico"
import type { AuthContextValue } from "@/contexts/AuthContext"
import { AuthContext } from "@/contexts/AuthContext"

vi.mock("@/api/profissionais", () => ({
  listarProfissionais: vi.fn(),
}))
vi.mock("@/api/servicos", async () => {
  const modulo = await vi.importActual<typeof import("@/api/servicos")>("@/api/servicos")
  return { ...modulo, criarServico: vi.fn() }
})

const { listarProfissionais } = await import("@/api/profissionais")
const { criarServico } = await import("@/api/servicos")

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

describe("EtapaServico", () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.mocked(listarProfissionais).mockResolvedValue([{ profissionalId: "prof-1", nome: "Ana Souza" }])
  })

  it("cria o primeiro serviço e atualiza o progresso do onboarding", async () => {
    vi.mocked(criarServico).mockResolvedValue({ servicoId: "servico-1" })
    const refrescarOrganizacao = vi.fn()
    const usuario = userEvent.setup()

    render(
      <AuthContext.Provider value={contextoAutenticado(refrescarOrganizacao)}>
        <EtapaServico />
      </AuthContext.Provider>,
    )

    await usuario.type(await screen.findByLabelText("Nome do serviço"), "Consulta inicial")
    await usuario.clear(screen.getByLabelText("Duração (minutos)"))
    await usuario.type(screen.getByLabelText("Duração (minutos)"), "50")
    await usuario.clear(screen.getByLabelText("Preço (R$)"))
    await usuario.type(screen.getByLabelText("Preço (R$)"), "250")

    await usuario.click(screen.getByRole("button", { name: "Concluir" }))

    await waitFor(() =>
      expect(criarServico).toHaveBeenCalledWith(
        "prof-1",
        { nome: "Consulta inicial", duracaoEmMinutos: 50, preco: 250, tipoAtendimento: "Online" },
        "token-de-teste",
      ),
    )
    expect(refrescarOrganizacao).toHaveBeenCalled()
  })
})
