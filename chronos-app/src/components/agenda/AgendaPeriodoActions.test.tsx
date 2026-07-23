import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { beforeEach, describe, expect, it, vi } from "vitest"

import type { PeriodoOcupado } from "@/api/agendamentos"
import { AgendaPeriodoActions } from "@/components/agenda/AgendaPeriodoActions"
import type { AuthContextValue } from "@/contexts/AuthContext"
import { AuthContext } from "@/contexts/AuthContext"

vi.mock("@/api/agendamentos", async () => {
  const modulo = await vi.importActual<typeof import("@/api/agendamentos")>("@/api/agendamentos")
  return { ...modulo, confirmarAgendamento: vi.fn(), cancelarAgendamento: vi.fn() }
})

const { confirmarAgendamento, cancelarAgendamento } = await import("@/api/agendamentos")

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

const PERIODO_PENDENTE: PeriodoOcupado = {
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

describe("AgendaPeriodoActions", () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it("confirma um agendamento pendente", async () => {
    vi.mocked(confirmarAgendamento).mockResolvedValue(undefined)
    const onAtualizado = vi.fn()
    const usuario = userEvent.setup()

    render(
      <AuthContext.Provider value={contextoAutenticado()}>
        <AgendaPeriodoActions periodo={PERIODO_PENDENTE} onReagendar={vi.fn()} onAtualizado={onAtualizado} />
      </AuthContext.Provider>,
    )

    await usuario.click(screen.getByRole("button", { name: "Confirmar agendamento de Maria Silva" }))

    await waitFor(() => expect(confirmarAgendamento).toHaveBeenCalledWith("agendamento-1", "token-de-teste"))
    expect(onAtualizado).toHaveBeenCalled()
  })

  it("não mostra o botão de confirmar para um agendamento já confirmado", () => {
    render(
      <AuthContext.Provider value={contextoAutenticado()}>
        <AgendaPeriodoActions periodo={{ ...PERIODO_PENDENTE, status: "Confirmado" }} onReagendar={vi.fn()} onAtualizado={vi.fn()} />
      </AuthContext.Provider>,
    )

    expect(screen.queryByRole("button", { name: "Confirmar agendamento de Maria Silva" })).not.toBeInTheDocument()
  })

  it("cancela um agendamento após confirmação no alert dialog", async () => {
    vi.mocked(cancelarAgendamento).mockResolvedValue(undefined)
    const onAtualizado = vi.fn()
    const usuario = userEvent.setup()

    render(
      <AuthContext.Provider value={contextoAutenticado()}>
        <AgendaPeriodoActions periodo={PERIODO_PENDENTE} onReagendar={vi.fn()} onAtualizado={onAtualizado} />
      </AuthContext.Provider>,
    )

    await usuario.click(screen.getByRole("button", { name: "Cancelar agendamento de Maria Silva" }))
    await usuario.click(await screen.findByRole("button", { name: "Cancelar agendamento" }))

    await waitFor(() => expect(cancelarAgendamento).toHaveBeenCalledWith("agendamento-1", "token-de-teste"))
    expect(onAtualizado).toHaveBeenCalled()
  })
})
