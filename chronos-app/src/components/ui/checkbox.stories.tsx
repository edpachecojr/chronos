import type { Meta, StoryObj } from "@storybook/react-vite"

import { Checkbox } from "./checkbox"
import { Label } from "./label"

const meta = {
  title: "Design System/Checkbox",
  component: Checkbox,
  parameters: { layout: "centered" },
} satisfies Meta<typeof Checkbox>

export default meta
type Story = StoryObj<typeof meta>

export const Default: Story = {
  render: (args) => (
    <div className="flex items-center gap-2">
      <Checkbox id="termos" {...args} />
      <Label htmlFor="termos">Aceito os termos de uso</Label>
    </div>
  ),
}

export const Marcado: Story = { ...Default, args: { defaultChecked: true } }

export const Desabilitado: Story = { ...Default, args: { disabled: true } }
