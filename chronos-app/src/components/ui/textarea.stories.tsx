import type { Meta, StoryObj } from "@storybook/react-vite"

import { Label } from "./label"
import { Textarea } from "./textarea"

const meta = {
  title: "Design System/Textarea",
  component: Textarea,
  parameters: { layout: "centered" },
  args: { placeholder: "Observações sobre o agendamento..." },
} satisfies Meta<typeof Textarea>

export default meta
type Story = StoryObj<typeof meta>

export const Default: Story = {
  render: (args) => (
    <div className="grid w-72 gap-1.5">
      <Label htmlFor="observacoes">Observações</Label>
      <Textarea id="observacoes" {...args} />
    </div>
  ),
}

export const Desabilitado: Story = { ...Default, args: { disabled: true, defaultValue: "Não editável" } }
