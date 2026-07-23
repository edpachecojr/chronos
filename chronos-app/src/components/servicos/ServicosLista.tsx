import { Pencil } from "lucide-react"

import type { ServicoResumo } from "@/api/servicos"
import { paraDuracaoEmMinutos } from "@/api/servicos"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"

const ROTULOS_TIPO_ATENDIMENTO: Record<ServicoResumo["tipoAtendimento"], string> = {
  Online: "Online",
  Domiciliar: "Domiciliar",
  NoEnderecoDoPrestador: "No endereço do prestador",
}

const FORMATADOR_MOEDA = new Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL" })

type ServicosListaProps = {
  servicos: ServicoResumo[]
  onEditar: (servico: ServicoResumo) => void
}

export function ServicosLista({ servicos, onEditar }: ServicosListaProps) {
  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead>Serviço</TableHead>
          <TableHead>Duração</TableHead>
          <TableHead>Preço</TableHead>
          <TableHead>Modalidade</TableHead>
          <TableHead className="w-0" />
        </TableRow>
      </TableHeader>
      <TableBody>
        {servicos.map((servico) => (
          <TableRow key={servico.servicoId}>
            <TableCell className="font-medium text-foreground">{servico.nome}</TableCell>
            <TableCell>{paraDuracaoEmMinutos(servico.duracao)} min</TableCell>
            <TableCell>{FORMATADOR_MOEDA.format(servico.preco)}</TableCell>
            <TableCell>
              <Badge variant="secondary">{ROTULOS_TIPO_ATENDIMENTO[servico.tipoAtendimento]}</Badge>
            </TableCell>
            <TableCell>
              <Button variant="ghost" size="icon-sm" onClick={() => onEditar(servico)}>
                <Pencil />
                <span className="sr-only">Editar {servico.nome}</span>
              </Button>
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  )
}
