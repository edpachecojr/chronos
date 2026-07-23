import { standardSchemaResolver } from "@hookform/resolvers/standard-schema"
import { CircleAlert } from "lucide-react"
import { useEffect, useState } from "react"
import { useForm } from "react-hook-form"

import {
  ROTULOS_DIA_DA_SEMANA,
  alterarDisponibilidade,
  criarDisponibilidade,
  traduzirErroDeDisponibilidade,
  type DisponibilidadeResumo,
} from "@/api/disponibilidades"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { Button } from "@/components/ui/button"
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog"
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form"
import { Input } from "@/components/ui/input"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { useAuth } from "@/hooks/useAuth"
import { esquemaDisponibilidade, type DisponibilidadeFormValores } from "@/lib/validation/disponibilidadeSchemas"

const VALORES_PADRAO: DisponibilidadeFormValores = { diaDaSemana: "Monday", inicio: "09:00", fim: "18:00" }

type DisponibilidadeDialogProps = {
  profissionalId: string
  disponibilidadeParaEditar?: DisponibilidadeResumo | null
  open: boolean
  onOpenChange: (aberto: boolean) => void
  onSalvo: () => void
}

export function DisponibilidadeDialog({
  profissionalId,
  disponibilidadeParaEditar,
  open,
  onOpenChange,
  onSalvo,
}: DisponibilidadeDialogProps) {
  const { accessToken } = useAuth()
  const [erro, setErro] = useState<string | null>(null)
  const form = useForm<DisponibilidadeFormValores>({
    resolver: standardSchemaResolver(esquemaDisponibilidade),
    defaultValues: VALORES_PADRAO,
  })

  useEffect(() => {
    if (!open) {
      return
    }
    setErro(null)
    form.reset(
      disponibilidadeParaEditar
        ? {
            diaDaSemana: disponibilidadeParaEditar.diaDaSemana,
            inicio: disponibilidadeParaEditar.inicio.slice(0, 5),
            fim: disponibilidadeParaEditar.fim.slice(0, 5),
          }
        : VALORES_PADRAO,
    )
  }, [open, disponibilidadeParaEditar, form])

  async function aoSubmeter(dados: DisponibilidadeFormValores) {
    if (!accessToken) {
      return
    }
    setErro(null)
    try {
      if (disponibilidadeParaEditar) {
        await alterarDisponibilidade(disponibilidadeParaEditar.disponibilidadeId, profissionalId, dados, accessToken)
      } else {
        await criarDisponibilidade(profissionalId, dados, accessToken)
      }
      onOpenChange(false)
      onSalvo()
    } catch (erroCapturado) {
      setErro(traduzirErroDeDisponibilidade(erroCapturado))
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{disponibilidadeParaEditar ? "Editar disponibilidade" : "Nova disponibilidade"}</DialogTitle>
          <DialogDescription>Defina o dia da semana e a janela de horário em que o profissional atende.</DialogDescription>
        </DialogHeader>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(aoSubmeter)} className="grid gap-4" noValidate>
            {erro && (
              <Alert variant="destructive" className="animate-in fade-in-0 slide-in-from-top-1 duration-200">
                <CircleAlert aria-hidden="true" />
                <AlertDescription>{erro}</AlertDescription>
              </Alert>
            )}
            <FormField
              control={form.control}
              name="diaDaSemana"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Dia da semana</FormLabel>
                  <Select value={field.value} onValueChange={field.onChange}>
                    <FormControl>
                      <SelectTrigger className="w-full">
                        <SelectValue placeholder="Selecione o dia" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {Object.entries(ROTULOS_DIA_DA_SEMANA).map(([valor, rotulo]) => (
                        <SelectItem key={valor} value={valor}>
                          {rotulo}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />
            <div className="grid grid-cols-2 gap-4">
              <FormField
                control={form.control}
                name="inicio"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Início</FormLabel>
                    <FormControl>
                      <Input type="time" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="fim"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Fim</FormLabel>
                    <FormControl>
                      <Input type="time" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>
            <DialogFooter>
              <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
                Cancelar
              </Button>
              <Button type="submit" disabled={form.formState.isSubmitting}>
                {form.formState.isSubmitting ? "Salvando..." : "Salvar"}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  )
}
