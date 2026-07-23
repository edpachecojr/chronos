import type { Meta, StoryObj } from "@storybook/react-vite"

import { Input } from "./input"
import { Label } from "./label"

const meta = {
  title: "Design System/Label",
  component: Label,
  parameters: { layout: "centered" },
} satisfies Meta<typeof Label>

export default meta
type Story = StoryObj<typeof meta>

export const Default: Story = { args: { children: "E-mail" } }

export const AssociadaAUmCampo: Story = {
  render: (args) => (
    <div className="grid w-64 gap-1.5">
      <Label htmlFor="email-exemplo" {...args} />
      <Input id="email-exemplo" type="email" placeholder="voce@exemplo.com" />
    </div>
  ),
  args: { children: "E-mail" },
}
