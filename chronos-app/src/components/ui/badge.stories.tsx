import type { Meta, StoryObj } from "@storybook/react-vite"

import { Badge } from "./badge"

const meta = {
  title: "Design System/Badge",
  component: Badge,
  parameters: { layout: "centered" },
  args: { children: "Confirmado" },
} satisfies Meta<typeof Badge>

export default meta
type Story = StoryObj<typeof meta>

export const Default: Story = {}

export const Secondary: Story = {
  args: { variant: "secondary", children: "Pendente" },
}

export const Destructive: Story = {
  args: { variant: "destructive", children: "Cancelado" },
}

export const Outline: Story = {
  args: { variant: "outline", children: "Rascunho" },
}
