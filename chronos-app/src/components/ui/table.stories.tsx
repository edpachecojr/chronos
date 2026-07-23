import type { Meta, StoryObj } from "@storybook/react-vite"

import { Badge } from "./badge"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "./table"

const meta = {
  title: "Design System/Table",
  component: Table,
  parameters: { layout: "centered" },
} satisfies Meta<typeof Table>

export default meta
type Story = StoryObj<typeof meta>

export const Default: Story = {
  render: (args) => (
    <Table {...args}>
      <TableHeader>
        <TableRow>
          <TableHead>Serviço</TableHead>
          <TableHead>Duração</TableHead>
          <TableHead>Preço</TableHead>
          <TableHead>Modalidade</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        <TableRow>
          <TableCell>Consulta inicial</TableCell>
          <TableCell>50 min</TableCell>
          <TableCell>R$ 250,00</TableCell>
          <TableCell>
            <Badge variant="secondary">Online</Badge>
          </TableCell>
        </TableRow>
        <TableRow>
          <TableCell>Corte de cabelo</TableCell>
          <TableCell>30 min</TableCell>
          <TableCell>R$ 60,00</TableCell>
          <TableCell>
            <Badge variant="outline">No endereço do prestador</Badge>
          </TableCell>
        </TableRow>
      </TableBody>
    </Table>
  ),
}
