import type { Meta, StoryObj } from "@storybook/react-vite"

import { Button } from "./button"

const meta = {
  title: "Design System/Button",
  component: Button,
  parameters: { layout: "centered" },
  args: { children: "Confirmar" },
} satisfies Meta<typeof Button>

export default meta
type Story = StoryObj<typeof meta>

export const Default: Story = {}

export const Secondary: Story = { args: { variant: "secondary" } }

export const Outline: Story = { args: { variant: "outline" } }

export const Ghost: Story = { args: { variant: "ghost" } }

export const Destructive: Story = {
  args: { variant: "destructive", children: "Cancelar agendamento" },
}

export const Disabled: Story = { args: { disabled: true } }
