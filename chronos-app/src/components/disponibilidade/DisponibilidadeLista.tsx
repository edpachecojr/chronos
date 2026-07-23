import { Pencil, Trash2 } from "lucide-react"

import { ROTULOS_DIA_DA_SEMANA, type DisponibilidadeResumo } from "@/api/disponibilidades"
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"

type DisponibilidadeListaProps = {
  disponibilidades: DisponibilidadeResumo[]
  onEditar: (disponibilidade: DisponibilidadeResumo) => void
  onRemover: (disponibilidade: DisponibilidadeResumo) => void
}

export function DisponibilidadeLista({ disponibilidades, onEditar, onRemover }: DisponibilidadeListaProps) {
  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead>Dia da semana</TableHead>
          <TableHead>Início</TableHead>
          <TableHead>Fim</TableHead>
          <TableHead className="w-0" />
        </TableRow>
      </TableHeader>
      <TableBody>
        {disponibilidades.map((disponibilidade) => (
          <TableRow key={disponibilidade.disponibilidadeId}>
            <TableCell>
              <Badge variant="secondary">{ROTULOS_DIA_DA_SEMANA[disponibilidade.diaDaSemana]}</Badge>
            </TableCell>
            <TableCell className="font-medium text-foreground">{disponibilidade.inicio.slice(0, 5)}</TableCell>
            <TableCell className="font-medium text-foreground">{disponibilidade.fim.slice(0, 5)}</TableCell>
            <TableCell className="flex gap-1">
              <Button variant="ghost" size="icon-sm" onClick={() => onEditar(disponibilidade)}>
                <Pencil />
                <span className="sr-only">Editar disponibilidade de {ROTULOS_DIA_DA_SEMANA[disponibilidade.diaDaSemana]}</span>
              </Button>
              <AlertDialog>
                <AlertDialogTrigger asChild>
                  <Button variant="ghost" size="icon-sm">
                    <Trash2 />
                    <span className="sr-only">
                      Remover disponibilidade de {ROTULOS_DIA_DA_SEMANA[disponibilidade.diaDaSemana]}
                    </span>
                  </Button>
                </AlertDialogTrigger>
                <AlertDialogContent>
                  <AlertDialogHeader>
                    <AlertDialogTitle>Remover esta disponibilidade?</AlertDialogTitle>
                    <AlertDialogDescription>
                      O profissional deixará de atender {ROTULOS_DIA_DA_SEMANA[disponibilidade.diaDaSemana].toLowerCase()} neste
                      horário. Esta ação não pode ser desfeita.
                    </AlertDialogDescription>
                  </AlertDialogHeader>
                  <AlertDialogFooter>
                    <AlertDialogCancel>Voltar</AlertDialogCancel>
                    <AlertDialogAction variant="destructive" onClick={() => onRemover(disponibilidade)}>
                      Remover
                    </AlertDialogAction>
                  </AlertDialogFooter>
                </AlertDialogContent>
              </AlertDialog>
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  )
}
