import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { beforeEach, describe, expect, it, vi } from "vitest"

import type { AuthContextValue } from "@/contexts/AuthContext"
import { AuthContext } from "@/contexts/AuthContext"
import { AgendamentoDialog } from "@/components/agendamentos/AgendamentoDialog"

vi.mock("@/api/agendamentos", async () => {
  const modulo = await vi.importActual<typeof import("@/api/agendamentos")>("@/api/agendamentos")
  return { ...modulo, criarAgendamento: vi.fn() }
})
vi.mock("@/api/servicos", async () => {
  const modulo = await vi.importActual<typeof import("@/api/servicos")>("@/api/servicos")
  return { ...modulo, listarServicos: vi.fn() }
})

const { criarAgendamento } = await import("@/api/agendamentos")
const { listarServicos } = await import("@/api/servicos")

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

describe("AgendamentoDialog", () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it("cria um agendamento com um serviço online, sem exigir endereço", async () => {
    vi.mocked(listarServicos).mockResolvedValue([
      { servicoId: "servico-1", nome: "Consulta inicial", duracao: "00:50:00", preco: 250, tipoAtendimento: "Online" },
    ])
    vi.mocked(criarAgendamento).mockResolvedValue({ agendamentoId: "agendamento-1" })
    const onSalvo = vi.fn()
    const usuario = userEvent.setup()

    render(
      <AuthContext.Provider value={contextoAutenticado()}>
        <AgendamentoDialog profissionalId="prof-1" open onOpenChange={() => {}} onSalvo={onSalvo} />
      </AuthContext.Provider>,
    )

    await waitFor(() => expect(listarServicos).toHaveBeenCalledWith("prof-1", "token-de-teste"))
    await usuario.click(screen.getByRole("combobox", { name: "Serviço" }))
    await usuario.click(await screen.findByRole("option", { name: "Consulta inicial" }))

    await usuario.type(screen.getByLabelText("Nome da pessoa atendida"), "Maria Silva")
    await usuario.type(screen.getByLabelText("Data"), "2026-07-27")
    await usuario.type(screen.getByLabelText("Horário"), "10:00")

    expect(screen.queryByLabelText("Endereço da pessoa atendida")).not.toBeInTheDocument()

    await usuario.click(screen.getByRole("button", { name: "Agendar" }))

    await waitFor(() =>
      expect(criarAgendamento).toHaveBeenCalledWith(
        "prof-1",
        expect.objectContaining({
          servicoId: "servico-1",
          nomePessoaAtendida: "Maria Silva",
          tipoPessoaAtendida: "Cliente",
          data: "2026-07-27",
          hora: "10:00",
        }),
        "token-de-teste",
      ),
    )
    expect(onSalvo).toHaveBeenCalled()
  })
})
