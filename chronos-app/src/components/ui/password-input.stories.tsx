import type { Meta, StoryObj } from "@storybook/react-vite"

import { Label } from "./label"
import { PasswordInput } from "./password-input"

const meta = {
  title: "Design System/Password Input",
  component: PasswordInput,
  parameters: { layout: "centered" },
} satisfies Meta<typeof PasswordInput>

export default meta
type Story = StoryObj<typeof meta>

export const Default: Story = {
  render: (args) => (
    <div className="grid w-64 gap-1.5">
      <Label htmlFor="senha">Senha</Label>
      <PasswordInput id="senha" {...args} />
    </div>
  ),
  args: { placeholder: "••••••••" },
}

export const ComValor: Story = {
  render: (args) => (
    <div className="grid w-64 gap-1.5">
      <Label htmlFor="senha-preenchida">Senha</Label>
      <PasswordInput id="senha-preenchida" {...args} />
    </div>
  ),
  args: { defaultValue: "SenhaForte123!" },
}
