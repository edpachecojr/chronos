import type { Meta, StoryObj } from "@storybook/react-vite"

import { Label } from "./label"
import { Switch } from "./switch"

const meta = {
  title: "Design System/Switch",
  component: Switch,
  parameters: { layout: "centered" },
} satisfies Meta<typeof Switch>

export default meta
type Story = StoryObj<typeof meta>

export const Default: Story = {
  render: (args) => (
    <div className="flex items-center gap-2">
      <Switch id="atendimento-domiciliar" {...args} />
      <Label htmlFor="atendimento-domiciliar">Atendimento domiciliar</Label>
    </div>
  ),
}

export const Marcado: Story = { ...Default, args: { defaultChecked: true } }

export const Desabilitado: Story = { ...Default, args: { disabled: true } }
