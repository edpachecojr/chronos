import type { Meta, StoryObj } from "@storybook/react-vite"
import { useState } from "react"

import { Calendar } from "./calendar"

const meta = {
  title: "Design System/Calendar",
  component: Calendar,
  parameters: { layout: "centered" },
} satisfies Meta<typeof Calendar>

export default meta
type Story = StoryObj<typeof meta>

export const Default: Story = {
  render: () => {
    function CalendarComSelecao() {
      const [selecionado, setSelecionado] = useState<Date | undefined>(new Date())
      return (
        <Calendar mode="single" selected={selecionado} onSelect={setSelecionado} className="rounded-md border" />
      )
    }
    return <CalendarComSelecao />
  },
}
