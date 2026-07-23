import type { Meta, StoryObj } from "@storybook/react-vite"

import { Label } from "./label"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "./select"

const meta = {
  title: "Design System/Select",
  component: Select,
  parameters: { layout: "centered" },
} satisfies Meta<typeof Select>

export default meta
type Story = StoryObj<typeof meta>

export const Default: Story = {
  render: (args) => (
    <div className="grid w-64 gap-1.5">
      <Label htmlFor="fuso-horario">Fuso horário</Label>
      <Select {...args}>
        <SelectTrigger id="fuso-horario" className="w-full">
          <SelectValue placeholder="Selecione um fuso horário" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value="america-sao_paulo">América/São Paulo (UTC-03:00)</SelectItem>
          <SelectItem value="america-manaus">América/Manaus (UTC-04:00)</SelectItem>
          <SelectItem value="america-noronha">América/Noronha (UTC-02:00)</SelectItem>
        </SelectContent>
      </Select>
    </div>
  ),
}

export const Desabilitado: Story = { ...Default, args: { disabled: true } }
