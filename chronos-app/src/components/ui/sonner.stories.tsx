import type { Meta, StoryObj } from "@storybook/react-vite"
import { toast } from "sonner"

import { Button } from "./button"
import { Toaster } from "./sonner"

const meta = {
  title: "Design System/Toaster",
  component: Toaster,
  parameters: { layout: "centered" },
} satisfies Meta<typeof Toaster>

export default meta
type Story = StoryObj<typeof meta>

export const Default: Story = {
  render: (args) => (
    <>
      <Button onClick={() => toast.success("Agendamento confirmado com sucesso.")}>Mostrar notificação</Button>
      <Toaster {...args} />
    </>
  ),
}
