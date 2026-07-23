import type { Meta, StoryObj } from "@storybook/react-vite"

import { Progress } from "./progress"

const meta = {
  title: "Design System/Progress",
  component: Progress,
  parameters: { layout: "centered" },
  args: { value: 50, "aria-label": "Progresso" },
} satisfies Meta<typeof Progress>

export default meta
type Story = StoryObj<typeof meta>

export const Default: Story = {
  render: (args) => (
    <div className="w-64">
      <Progress {...args} />
    </div>
  ),
}

export const Inicio: Story = { ...Default, args: { value: 25 } }

export const Concluido: Story = { ...Default, args: { value: 100 } }
