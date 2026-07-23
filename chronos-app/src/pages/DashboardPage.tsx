import { Plus } from "lucide-react"
import { useCallback, useEffect, useState } from "react"

import { consultarAgendaDiaria, traduzirErroDeAgendamento, type AgendaDiariaResultado, type PeriodoOcupado } from "@/api/agendamentos"
import { AgendaDataSeletor } from "@/components/agenda/AgendaDataSeletor"
import { AgendaDiaGrade } from "@/components/agenda/AgendaDiaGrade"
import { AgendamentoDialog } from "@/components/agendamentos/AgendamentoDialog"
import { ReagendarDialog } from "@/components/agendamentos/ReagendarDialog"
import { EstadoCarregando } from "@/components/estado/EstadoCarregando"
import { EstadoErro } from "@/components/estado/EstadoErro"
import { Button } from "@/components/ui/button"
import { useAuth } from "@/hooks/useAuth"
import { useProfissionalAtual } from "@/hooks/useProfissionalAtual"

function paraDataApi(data: Date): string {
  const ano = data.getFullYear()
  const mes = String(data.getMonth() + 1).padStart(2, "0")
  const dia = String(data.getDate()).padStart(2, "0")
  return `${ano}-${mes}-${dia}`
}

export function DashboardPage() {
  const { accessToken } = useAuth()
  const { profissionalId, carregando: carregandoProfissional, erro: erroProfissional } = useProfissionalAtual()
  const [data, setData] = useState(() => new Date())
  const [agenda, setAgenda] = useState<AgendaDiariaResultado | null>(null)
  const [carregandoAgenda, setCarregandoAgenda] = useState(true)
  const [erro, setErro] = useState<string | null>(null)
  const [dialogAberto, setDialogAberto] = useState(false)
  const [periodoParaReagendar, setPeriodoParaReagendar] = useState<PeriodoOcupado | null>(null)

  const carregarAgenda = useCallback(async () => {
    if (!accessToken || !profissionalId) {
      return
    }
    setCarregandoAgenda(true)
    setErro(null)
    try {
      const resultado = await consultarAgendaDiaria(profissionalId, paraDataApi(data), accessToken)
      setAgenda(resultado)
    } catch (erroCapturado) {
      setErro(traduzirErroDeAgendamento(erroCapturado))
    } finally {
      setCarregandoAgenda(false)
    }
  }, [accessToken, profissionalId, data])

  useEffect(() => {
    void carregarAgenda()
  }, [carregarAgenda])

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center justify-between">
        <div className="flex flex-col gap-1">
          <h1 className="text-2xl font-semibold tracking-tight text-foreground">Agenda</h1>
          <p className="text-sm text-muted-foreground">Consulte os agendamentos e a disponibilidade do profissional.</p>
        </div>
        {profissionalId && (
          <Button onClick={() => setDialogAberto(true)}>
            <Plus />
            Novo agendamento
          </Button>
        )}
      </div>

      <AgendaDataSeletor data={data} onDataChange={setData} />

      {(carregandoProfissional || carregandoAgenda) && !erro && !erroProfissional && <EstadoCarregando />}

      {(erro || erroProfissional) && <EstadoErro mensagem={erro ?? erroProfissional!} aoTentarNovamente={carregarAgenda} />}

      {!carregandoAgenda && !erro && !erroProfissional && agenda && (
        <AgendaDiaGrade agenda={agenda} onReagendar={setPeriodoParaReagendar} onAtualizado={carregarAgenda} />
      )}

      {profissionalId && (
        <AgendamentoDialog
          profissionalId={profissionalId}
          open={dialogAberto}
          onOpenChange={setDialogAberto}
          onSalvo={carregarAgenda}
        />
      )}

      {profissionalId && (
        <ReagendarDialog
          profissionalId={profissionalId}
          data={data}
          periodo={periodoParaReagendar}
          onOpenChange={(aberto) => !aberto && setPeriodoParaReagendar(null)}
          onSalvo={carregarAgenda}
        />
      )}
    </div>
  )
}
