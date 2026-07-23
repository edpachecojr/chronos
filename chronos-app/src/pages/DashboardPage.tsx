import { CalendarClock } from "lucide-react"

import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardAction,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"

export function DashboardPage() {
  return (
    <div className="flex flex-col gap-4">
      <h1 className="text-2xl font-semibold tracking-tight text-foreground">Agenda</h1>
      <Card className="w-full max-w-sm">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <CalendarClock className="size-5 text-brand-500" aria-hidden="true" />
            Próximo agendamento
          </CardTitle>
          <CardDescription>Corte de cabelo com Ana Souza</CardDescription>
          <CardAction>
            <Badge variant="secondary">Confirmado</Badge>
          </CardAction>
        </CardHeader>
        <CardContent>
          <p className="text-sm text-muted-foreground">
            Quinta-feira, 23 de julho às 14h30, na unidade Centro.
          </p>
        </CardContent>
        <CardFooter className="gap-2">
          <Button variant="outline" className="flex-1">
            Remarcar
          </Button>
          <Button className="flex-1">Confirmar</Button>
        </CardFooter>
      </Card>
    </div>
  )
}
