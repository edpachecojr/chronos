import { CalendarClock, Check, X } from "lucide-react"
import { toast } from "sonner"

import { cancelarAgendamento, confirmarAgendamento, traduzirErroDeAgendamento, type PeriodoOcupado } from "@/api/agendamentos"
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog"
import { Button } from "@/components/ui/button"
import { useAuth } from "@/hooks/useAuth"

type AgendaPeriodoActionsProps = {
  periodo: PeriodoOcupado
  onReagendar: () => void
  onAtualizado: () => void
}

export function AgendaPeriodoActions({ periodo, onReagendar, onAtualizado }: AgendaPeriodoActionsProps) {
  const { accessToken } = useAuth()

  async function confirmar() {
    if (!accessToken) {
      return
    }
    try {
      await confirmarAgendamento(periodo.agendamentoId, accessToken)
      toast.success("Agendamento confirmado.")
      onAtualizado()
    } catch (erroCapturado) {
      toast.error(traduzirErroDeAgendamento(erroCapturado))
    }
  }

  async function cancelar() {
    if (!accessToken) {
      return
    }
    try {
      await cancelarAgendamento(periodo.agendamentoId, accessToken)
      toast.success("Agendamento cancelado.")
      onAtualizado()
    } catch (erroCapturado) {
      toast.error(traduzirErroDeAgendamento(erroCapturado))
    }
  }

  return (
    <div className="flex items-center gap-1">
      {periodo.status === "Pendente" && (
        <Button variant="ghost" size="icon-sm" onClick={confirmar}>
          <Check />
          <span className="sr-only">Confirmar agendamento de {periodo.nomePessoaAtendida}</span>
        </Button>
      )}
      <Button variant="ghost" size="icon-sm" onClick={onReagendar}>
        <CalendarClock />
        <span className="sr-only">Reagendar agendamento de {periodo.nomePessoaAtendida}</span>
      </Button>
      <AlertDialog>
        <AlertDialogTrigger asChild>
          <Button variant="ghost" size="icon-sm">
            <X />
            <span className="sr-only">Cancelar agendamento de {periodo.nomePessoaAtendida}</span>
          </Button>
        </AlertDialogTrigger>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Cancelar este agendamento?</AlertDialogTitle>
            <AlertDialogDescription>
              O horário de {periodo.nomePessoaAtendida} ficará livre novamente. Esta ação não pode ser desfeita.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Voltar</AlertDialogCancel>
            <AlertDialogAction variant="destructive" onClick={cancelar}>
              Cancelar agendamento
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}
