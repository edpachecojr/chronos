import { format } from "date-fns"
import { ptBR } from "date-fns/locale"
import { ChevronLeft, ChevronRight } from "lucide-react"

import { Button } from "@/components/ui/button"
import { Calendar } from "@/components/ui/calendar"
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover"

type AgendaDataSeletorProps = {
  data: Date
  onDataChange: (data: Date) => void
}

function comPrimeiraLetraMaiuscula(texto: string): string {
  return texto.charAt(0).toUpperCase() + texto.slice(1)
}

export function AgendaDataSeletor({ data, onDataChange }: AgendaDataSeletorProps) {
  function deslocarDias(quantidade: number) {
    const novaData = new Date(data)
    novaData.setDate(novaData.getDate() + quantidade)
    onDataChange(novaData)
  }

  return (
    <div className="flex items-center gap-2">
      <Button variant="outline" size="icon-sm" onClick={() => deslocarDias(-1)}>
        <ChevronLeft />
        <span className="sr-only">Dia anterior</span>
      </Button>
      <Popover>
        <PopoverTrigger asChild>
          <Button variant="outline" className="min-w-56 justify-start">
            {comPrimeiraLetraMaiuscula(format(data, "EEEE, d 'de' MMMM", { locale: ptBR }))}
          </Button>
        </PopoverTrigger>
        <PopoverContent className="w-auto p-0">
          <Calendar mode="single" selected={data} onSelect={(novaData) => novaData && onDataChange(novaData)} locale={ptBR} />
        </PopoverContent>
      </Popover>
      <Button variant="outline" size="icon-sm" onClick={() => deslocarDias(1)}>
        <ChevronRight />
        <span className="sr-only">Próximo dia</span>
      </Button>
      <Button variant="ghost" onClick={() => onDataChange(new Date())}>
        Hoje
      </Button>
    </div>
  )
}
