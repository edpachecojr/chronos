import type { Meta, StoryObj } from "@storybook/react-vite"

import { ColorTokens } from "./ColorTokens"
import { brandScale, dangerScale, neutralScale } from "./tokens"

const meta = {
  title: "Design System/Tokens/Cores",
  component: ColorTokens,
  parameters: { layout: "fullscreen" },
} satisfies Meta<typeof ColorTokens>

export default meta
type Story = StoryObj<typeof meta>

export const Escalas: Story = {
  args: {
    sections: [
      { title: "Brand (derivada da Key Color #2563EB)", tones: brandScale },
      { title: "Neutral (superfícies, bordas e texto)", tones: neutralScale },
      { title: "Danger (ações destrutivas)", tones: dangerScale },
    ],
  },
}
