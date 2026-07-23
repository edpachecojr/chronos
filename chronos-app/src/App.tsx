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

function App() {
  return (
    <main className="flex min-h-svh items-center justify-center bg-background p-6">
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
    </main>
  )
}

export default App
