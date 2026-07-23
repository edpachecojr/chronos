import { CircleAlert } from "lucide-react"

import { Alert, AlertDescription } from "@/components/ui/alert"
import { Button } from "@/components/ui/button"

type EstadoErroProps = {
  mensagem: string
  aoTentarNovamente?: () => void
}

export function EstadoErro({ mensagem, aoTentarNovamente }: EstadoErroProps) {
  return (
    <Alert variant="destructive" className="animate-in fade-in-0 slide-in-from-top-1 duration-200">
      <CircleAlert aria-hidden="true" />
      <AlertDescription className="flex flex-1 items-center justify-between gap-4">
        <span>{mensagem}</span>
        {aoTentarNovamente && (
          <Button variant="outline" size="sm" onClick={aoTentarNovamente}>
            Tentar novamente
          </Button>
        )}
      </AlertDescription>
    </Alert>
  )
}
