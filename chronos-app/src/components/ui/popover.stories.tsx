import type { Meta, StoryObj } from "@storybook/react-vite"

import { Button } from "./button"
import { Popover, PopoverContent, PopoverTrigger } from "./popover"

const meta = {
  title: "Design System/Popover",
  component: Popover,
  parameters: { layout: "centered" },
} satisfies Meta<typeof Popover>

export default meta
type Story = StoryObj<typeof meta>

export const Default: Story = {
  render: (args) => (
    <Popover {...args}>
      <PopoverTrigger asChild>
        <Button variant="outline">Selecionar dia</Button>
      </PopoverTrigger>
      <PopoverContent>
        <p className="text-sm">Conteúdo do popover, como um seletor de data.</p>
      </PopoverContent>
    </Popover>
  ),
}
