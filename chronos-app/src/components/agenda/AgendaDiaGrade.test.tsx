import { render, screen } from "@testing-library/react"
import type { ReactElement } from "react"
import { describe, expect, it, vi } from "vitest"

import type { AgendaDiariaResultado } from "@/api/agendamentos"
import { AgendaDiaGrade } from "@/components/agenda/AgendaDiaGrade"
import type { AuthContextValue } from "@/contexts/AuthContext"
import { AuthContext } from "@/contexts/AuthContext"

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

function renderComAuth(ui: ReactElement) {
  return render(<AuthContext.Provider value={contextoAutenticado()}>{ui}</AuthContext.Provider>)
}

describe("AgendaDiaGrade", () => {
  it("renderiza janelas de disponibilidade e períodos ocupados", () => {
    const agenda: AgendaDiariaResultado = {
      data: "2026-07-27",
      diaDaSemana: "Monday",
      janelasDisponiveis: [{ inicio: "09:00:00", fim: "18:00:00" }],
      periodosOcupados: [
        {
          agendamentoId: "agendamento-1",
          servicoId: "servico-1",
          inicio: "10:00:00",
          fim: "10:50:00",
          status: "Confirmado",
          nomeServico: "Consulta inicial",
          nomePessoaAtendida: "Maria Silva",
          tipoPessoaAtendida: "Paciente",
          enderecoPessoaAtendida: null,
        },
      ],
    }

    renderComAuth(<AgendaDiaGrade agenda={agenda} onReagendar={vi.fn()} onAtualizado={vi.fn()} />)

    expect(screen.getByText("09:00 – 18:00")).toBeInTheDocument()
    expect(screen.getByText("10:00 – 10:50")).toBeInTheDocument()
    expect(screen.getByText("Consulta inicial · Maria Silva")).toBeInTheDocument()
    expect(screen.getByText("Confirmado")).toBeInTheDocument()
  })

  it("mostra o estado vazio quando não há agendamentos no dia", () => {
    const agenda: AgendaDiariaResultado = {
      data: "2026-07-27",
      diaDaSemana: "Monday",
      janelasDisponiveis: [],
      periodosOcupados: [],
    }

    renderComAuth(<AgendaDiaGrade agenda={agenda} onReagendar={vi.fn()} onAtualizado={vi.fn()} />)

    expect(screen.getByText("Nenhum agendamento neste dia")).toBeInTheDocument()
    expect(screen.getByText("Nenhuma janela de disponibilidade configurada para este dia.")).toBeInTheDocument()
  })
})
