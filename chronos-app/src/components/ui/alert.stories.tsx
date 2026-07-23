import type { Meta, StoryObj } from "@storybook/react-vite"
import { CircleAlert } from "lucide-react"

import { Alert, AlertDescription, AlertTitle } from "./alert"

const meta = {
  title: "Design System/Alert",
  component: Alert,
  parameters: { layout: "centered" },
} satisfies Meta<typeof Alert>

export default meta
type Story = StoryObj<typeof meta>

export const Default: Story = {
  render: (args) => (
    <Alert {...args} className="w-80">
      <CircleAlert aria-hidden="true" />
      <AlertTitle>Não foi possível entrar</AlertTitle>
      <AlertDescription>Verifique seu e-mail e senha e tente novamente.</AlertDescription>
    </Alert>
  ),
}

export const Destructive: Story = { ...Default, args: { variant: "destructive" } }
