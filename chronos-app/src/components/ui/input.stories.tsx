import type { Meta, StoryObj } from "@storybook/react-vite"
import { Search } from "lucide-react"

import { Input } from "./input"
import { Label } from "./label"

const meta = {
  title: "Design System/Input",
  component: Input,
  parameters: { layout: "centered" },
  args: { placeholder: "voce@exemplo.com" },
} satisfies Meta<typeof Input>

export default meta
type Story = StoryObj<typeof meta>

export const Texto: Story = { args: { type: "text", placeholder: "Ana Souza" } }

export const Email: Story = { args: { type: "email" } }

export const Numero: Story = { args: { type: "number", placeholder: "0" } }

export const Busca: Story = {
  render: (args) => (
    <div className="relative w-64">
      <Search
        className="pointer-events-none absolute top-1/2 left-2.5 size-4 -translate-y-1/2 text-muted-foreground"
        aria-hidden="true"
      />
      <Input {...args} className="pl-8" />
    </div>
  ),
  args: { type: "search", placeholder: "Buscar serviços..." },
}

export const Invalido: Story = {
  render: (args) => (
    <div className="grid w-64 gap-1.5">
      <Label htmlFor="input-invalido">E-mail</Label>
      <Input id="input-invalido" {...args} />
      <p className="text-sm text-destructive">Informe um e-mail válido.</p>
    </div>
  ),
  args: { type: "email", defaultValue: "email-invalido", "aria-invalid": true },
}

export const Desabilitado: Story = { args: { disabled: true, defaultValue: "Não editável" } }
