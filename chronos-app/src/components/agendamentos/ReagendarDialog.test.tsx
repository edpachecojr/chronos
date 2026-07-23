import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { beforeEach, describe, expect, it, vi } from "vitest"

import type { PeriodoOcupado } from "@/api/agendamentos"
import { ReagendarDialog } from "@/components/agendamentos/ReagendarDialog"
import type { AuthContextValue } from "@/contexts/AuthContext"
import { AuthContext } from "@/contexts/AuthContext"

vi.mock("@/api/agendamentos", async () => {
  const modulo = await vi.importActual<typeof import("@/api/agendamentos")>("@/api/agendamentos")
  return { ...modulo, reagendarAgendamento: vi.fn() }
})
vi.mock("@/api/servicos", async () => {
  const modulo = await vi.importActual<typeof import("@/api/servicos")>("@/api/servicos")
  return { ...modulo, listarServicos: vi.fn() }
})

const { reagendarAgendamento } = await import("@/api/agendamentos")
const { listarServicos } = await import("@/api/servicos")

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

const PERIODO: PeriodoOcupado = {
  agendamentoId: "agendamento-1",
  servicoId: "servico-1",
  inicio: "10:00:00",
  fim: "10:50:00",
  status: "Pendente",
  nomeServico: "Consulta inicial",
  nomePessoaAtendida: "Maria Silva",
  tipoPessoaAtendida: "Paciente",
  enderecoPessoaAtendida: null,
}

describe("ReagendarDialog", () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it("pré-preenche o formulário e reagenda mantendo o serviço original", async () => {
    vi.mocked(listarServicos).mockResolvedValue([
      { servicoId: "servico-1", nome: "Consulta inicial", duracao: "00:50:00", preco: 250, tipoAtendimento: "Online" },
    ])
    vi.mocked(reagendarAgendamento).mockResolvedValue(undefined)
    const onSalvo = vi.fn()
    const usuario = userEvent.setup()

    render(
      <AuthContext.Provider value={contextoAutenticado()}>
        <ReagendarDialog profissionalId="prof-1" data={new Date(2026, 6, 27)} periodo={PERIODO} onOpenChange={() => {}} onSalvo={onSalvo} />
      </AuthContext.Provider>,
    )

    expect(await screen.findByDisplayValue("Maria Silva")).toBeInTheDocument()
    expect(screen.getByDisplayValue("2026-07-27")).toBeInTheDocument()
    expect(screen.getByDisplayValue("10:00")).toBeInTheDocument()

    await usuario.clear(screen.getByLabelText("Horário"))
    await usuario.type(screen.getByLabelText("Horário"), "11:00")
    await usuario.click(screen.getByRole("button", { name: "Salvar" }))

    await waitFor(() =>
      expect(reagendarAgendamento).toHaveBeenCalledWith(
        "agendamento-1",
        "prof-1",
        "servico-1",
        expect.objectContaining({ nomePessoaAtendida: "Maria Silva", hora: "11:00" }),
        "token-de-teste",
      ),
    )
    expect(onSalvo).toHaveBeenCalled()
  })
})
