import { CalendarClock, LogOut } from "lucide-react"

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
import { useAuth } from "@/hooks/useAuth"

export function DashboardPage() {
  const { organizacao, sair } = useAuth()

  return (
    <main className="flex min-h-svh flex-col items-center justify-center gap-4 bg-background p-6">
      <div className="flex w-full max-w-sm items-center justify-between">
        <p className="text-sm font-medium text-foreground">{organizacao?.nome}</p>
        <Button variant="ghost" size="sm" onClick={sair}>
          <LogOut aria-hidden="true" />
          Sair
        </Button>
      </div>
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
