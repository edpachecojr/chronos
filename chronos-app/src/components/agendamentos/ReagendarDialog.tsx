import { standardSchemaResolver } from "@hookform/resolvers/standard-schema"
import { CircleAlert } from "lucide-react"
import { useEffect, useMemo, useState } from "react"
import { useForm } from "react-hook-form"

import { reagendarAgendamento, traduzirErroDeAgendamento, type PeriodoOcupado } from "@/api/agendamentos"
import { listarServicos } from "@/api/servicos"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { Button } from "@/components/ui/button"
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog"
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form"
import { Input } from "@/components/ui/input"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { useAuth } from "@/hooks/useAuth"
import { criarEsquemaReagendamento, type ReagendamentoFormValores } from "@/lib/validation/agendamentoSchemas"

const ROTULOS_TIPO_PESSOA_ATENDIDA: Record<ReagendamentoFormValores["tipoPessoaAtendida"], string> = {
  Cliente: "Cliente",
  Paciente: "Paciente",
  Aluno: "Aluno",
  Outro: "Outro",
}

const VALORES_PADRAO: ReagendamentoFormValores = {
  nomePessoaAtendida: "",
  tipoPessoaAtendida: "Cliente",
  data: "",
  hora: "",
  enderecoPessoaAtendida: "",
}

function paraDataFormulario(data: Date): string {
  const ano = data.getFullYear()
  const mes = String(data.getMonth() + 1).padStart(2, "0")
  const dia = String(data.getDate()).padStart(2, "0")
  return `${ano}-${mes}-${dia}`
}

type ReagendarDialogProps = {
  profissionalId: string
  /** Dia atualmente exibido na agenda — o `periodo.inicio` só tem o horário, não a data. */
  data: Date
  periodo: PeriodoOcupado | null
  onOpenChange: (aberto: boolean) => void
  onSalvo: () => void
}

export function ReagendarDialog({ profissionalId, data, periodo, onOpenChange, onSalvo }: ReagendarDialogProps) {
  const { accessToken } = useAuth()
  const [exigeEndereco, setExigeEndereco] = useState(false)
  const [erro, setErro] = useState<string | null>(null)
  const esquema = useMemo(() => criarEsquemaReagendamento(exigeEndereco), [exigeEndereco])
  const form = useForm<ReagendamentoFormValores>({
    resolver: standardSchemaResolver(esquema),
    defaultValues: VALORES_PADRAO,
  })

  useEffect(() => {
    if (!periodo || !accessToken) {
      return
    }
    setErro(null)
    form.reset({
      nomePessoaAtendida: periodo.nomePessoaAtendida,
      tipoPessoaAtendida: periodo.tipoPessoaAtendida,
      data: paraDataFormulario(data),
      hora: periodo.inicio.slice(0, 5),
      enderecoPessoaAtendida: periodo.enderecoPessoaAtendida ?? "",
    })
    void listarServicos(profissionalId, accessToken).then((servicos) => {
      const servico = servicos.find((item) => item.servicoId === periodo.servicoId)
      setExigeEndereco(servico?.tipoAtendimento === "Domiciliar")
    })
  }, [periodo, accessToken, profissionalId, data, form])

  async function aoSubmeter(dados: ReagendamentoFormValores) {
    if (!accessToken || !periodo) {
      return
    }
    setErro(null)
    try {
      await reagendarAgendamento(periodo.agendamentoId, profissionalId, periodo.servicoId, dados, accessToken)
      onOpenChange(false)
      onSalvo()
    } catch (erroCapturado) {
      setErro(traduzirErroDeAgendamento(erroCapturado))
    }
  }

  return (
    <Dialog open={periodo !== null} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Reagendar {periodo?.nomeServico}</DialogTitle>
          <DialogDescription>O serviço e o profissional não podem ser alterados ao reagendar.</DialogDescription>
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
              name="nomePessoaAtendida"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Nome da pessoa atendida</FormLabel>
                  <FormControl>
                    <Input {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="tipoPessoaAtendida"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Tipo</FormLabel>
                  <Select value={field.value} onValueChange={field.onChange}>
                    <FormControl>
                      <SelectTrigger className="w-full">
                        <SelectValue />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {Object.entries(ROTULOS_TIPO_PESSOA_ATENDIDA).map(([valor, rotulo]) => (
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
                name="data"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Data</FormLabel>
                    <FormControl>
                      <Input type="date" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="hora"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Horário</FormLabel>
                    <FormControl>
                      <Input type="time" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>
            {exigeEndereco && (
              <FormField
                control={form.control}
                name="enderecoPessoaAtendida"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Endereço da pessoa atendida</FormLabel>
                    <FormControl>
                      <Input {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            )}
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
