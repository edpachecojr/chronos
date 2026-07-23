import type { AgendaDiariaResultado, PeriodoOcupado, StatusAgendamento } from "@/api/agendamentos"
import { AgendaPeriodoActions } from "@/components/agenda/AgendaPeriodoActions"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { EstadoVazio } from "@/components/estado/EstadoVazio"

const ROTULOS_STATUS: Record<StatusAgendamento, { rotulo: string; variante: "secondary" | "default" | "outline" }> = {
  Pendente: { rotulo: "Pendente", variante: "secondary" },
  Confirmado: { rotulo: "Confirmado", variante: "default" },
  Cancelado: { rotulo: "Cancelado", variante: "outline" },
}

function formatarHorario(horario: string): string {
  return horario.slice(0, 5)
}

type AgendaDiaGradeProps = {
  agenda: AgendaDiariaResultado
  onReagendar: (periodo: PeriodoOcupado) => void
  onAtualizado: () => void
}

export function AgendaDiaGrade({ agenda, onReagendar, onAtualizado }: AgendaDiaGradeProps) {
  const periodos = [...agenda.periodosOcupados].sort((a, b) => a.inicio.localeCompare(b.inicio))

  return (
    <div className="flex flex-col gap-4">
      <Card>
        <CardHeader>
          <CardTitle className="text-sm font-medium">Disponibilidade do dia</CardTitle>
        </CardHeader>
        <CardContent className="flex flex-wrap gap-2">
          {agenda.janelasDisponiveis.length === 0 ? (
            <p className="text-sm text-muted-foreground">Nenhuma janela de disponibilidade configurada para este dia.</p>
          ) : (
            agenda.janelasDisponiveis.map((janela) => (
              <Badge key={`${janela.inicio}-${janela.fim}`} variant="outline">
                {formatarHorario(janela.inicio)} – {formatarHorario(janela.fim)}
              </Badge>
            ))
          )}
        </CardContent>
      </Card>

      {periodos.length === 0 ? (
        <EstadoVazio titulo="Nenhum agendamento neste dia" descricao="Os agendamentos criados para este dia aparecerão aqui." />
      ) : (
        <div className="flex flex-col gap-2">
          {periodos.map((periodo) => (
            <Card key={periodo.agendamentoId}>
              <CardContent className="flex items-center justify-between gap-4">
                <div className="flex flex-col gap-0.5">
                  <span className="font-medium text-foreground">
                    {formatarHorario(periodo.inicio)} – {formatarHorario(periodo.fim)}
                  </span>
                  <span className="text-sm text-muted-foreground">
                    {periodo.nomeServico} · {periodo.nomePessoaAtendida}
                  </span>
                </div>
                <div className="flex items-center gap-3">
                  <Badge variant={ROTULOS_STATUS[periodo.status].variante}>{ROTULOS_STATUS[periodo.status].rotulo}</Badge>
                  <AgendaPeriodoActions
                    periodo={periodo}
                    onReagendar={() => onReagendar(periodo)}
                    onAtualizado={onAtualizado}
                  />
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      )}
    </div>
  )
}
