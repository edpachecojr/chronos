import type { Meta, StoryObj } from "@storybook/react-vite"

import { Label } from "./label"
import { RadioGroup, RadioGroupItem } from "./radio-group"

const meta = {
  title: "Design System/Radio Group",
  component: RadioGroup,
  parameters: { layout: "centered" },
} satisfies Meta<typeof RadioGroup>

export default meta
type Story = StoryObj<typeof meta>

export const Default: Story = {
  render: (args) => (
    <RadioGroup {...args} className="w-64" defaultValue="diario">
      <div className="flex items-center gap-2">
        <RadioGroupItem id="lembrete-diario" value="diario" />
        <Label htmlFor="lembrete-diario">Lembrete diário</Label>
      </div>
      <div className="flex items-center gap-2">
        <RadioGroupItem id="lembrete-semanal" value="semanal" />
        <Label htmlFor="lembrete-semanal">Lembrete semanal</Label>
      </div>
      <div className="flex items-center gap-2">
        <RadioGroupItem id="lembrete-nunca" value="nunca" />
        <Label htmlFor="lembrete-nunca">Nunca notificar</Label>
      </div>
    </RadioGroup>
  ),
}

export const ComItemDesabilitado: Story = {
  render: (args) => (
    <RadioGroup {...args} className="w-64" defaultValue="diario">
      <div className="flex items-center gap-2">
        <RadioGroupItem id="opcao-diario" value="diario" />
        <Label htmlFor="opcao-diario">Lembrete diário</Label>
      </div>
      <div className="flex items-center gap-2">
        <RadioGroupItem id="opcao-semanal" value="semanal" disabled />
        <Label htmlFor="opcao-semanal">Lembrete semanal (indisponível)</Label>
      </div>
    </RadioGroup>
  ),
}
