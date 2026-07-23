import type { Meta, StoryObj } from "@storybook/react-vite"

import { Separator } from "./separator"

const meta = {
  title: "Design System/Separator",
  component: Separator,
  parameters: { layout: "centered" },
} satisfies Meta<typeof Separator>

export default meta
type Story = StoryObj<typeof meta>

export const Horizontal: Story = {
  render: (args) => (
    <div className="w-64 text-sm text-muted-foreground">
      <p>Entrar com e-mail e senha</p>
      <Separator {...args} className="my-3" />
      <p>Ainda não tem conta? Cadastre-se.</p>
    </div>
  ),
}

export const Vertical: Story = {
  render: (args) => (
    <div className="flex h-8 items-center gap-3 text-sm">
      <span>Perfil</span>
      <Separator {...args} orientation="vertical" />
      <span>Configurações</span>
    </div>
  ),
}
