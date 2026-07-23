import { render, screen } from "@testing-library/react"
import { describe, expect, it } from "vitest"

import type { AgendaDiariaResultado } from "@/api/agendamentos"
import { AgendaDiaGrade } from "@/components/agenda/AgendaDiaGrade"

describe("AgendaDiaGrade", () => {
  it("renderiza janelas de disponibilidade e períodos ocupados", () => {
    const agenda: AgendaDiariaResultado = {
      data: "2026-07-27",
      diaDaSemana: "Monday",
      janelasDisponiveis: [{ inicio: "09:00:00", fim: "18:00:00" }],
      periodosOcupados: [
        {
          agendamentoId: "agendamento-1",
          inicio: "10:00:00",
          fim: "10:50:00",
          status: "Confirmado",
          nomeServico: "Consulta inicial",
          nomePessoaAtendida: "Maria Silva",
        },
      ],
    }

    render(<AgendaDiaGrade agenda={agenda} />)

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

    render(<AgendaDiaGrade agenda={agenda} />)

    expect(screen.getByText("Nenhum agendamento neste dia")).toBeInTheDocument()
    expect(screen.getByText("Nenhuma janela de disponibilidade configurada para este dia.")).toBeInTheDocument()
  })
})
