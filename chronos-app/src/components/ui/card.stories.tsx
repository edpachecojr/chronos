import type { Meta, StoryObj } from "@storybook/react-vite"
import { CalendarClock } from "lucide-react"

import { Badge } from "./badge"
import { Button } from "./button"
import {
  Card,
  CardAction,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "./card"

const meta = {
  title: "Design System/Card",
  component: Card,
  parameters: { layout: "centered" },
} satisfies Meta<typeof Card>

export default meta
type Story = StoryObj<typeof meta>

export const Agendamento: Story = {
  render: () => (
    <Card className="w-sm">
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
  ),
}
