import { standardSchemaResolver } from "@hookform/resolvers/standard-schema"
import { CircleAlert } from "lucide-react"
import { useEffect, useMemo, useState } from "react"
import { useForm } from "react-hook-form"

import { criarAgendamento, traduzirErroDeAgendamento } from "@/api/agendamentos"
import { listarServicos, type ServicoResumo, type TipoAtendimento } from "@/api/servicos"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { Button } from "@/components/ui/button"
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog"
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form"
import { Input } from "@/components/ui/input"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { useAuth } from "@/hooks/useAuth"
import { criarEsquemaAgendamento, type AgendamentoFormValores } from "@/lib/validation/agendamentoSchemas"

const ROTULOS_TIPO_PESSOA_ATENDIDA: Record<AgendamentoFormValores["tipoPessoaAtendida"], string> = {
  Cliente: "Cliente",
  Paciente: "Paciente",
  Aluno: "Aluno",
  Outro: "Outro",
}

const VALORES_PADRAO: AgendamentoFormValores = {
  servicoId: "",
  nomePessoaAtendida: "",
  tipoPessoaAtendida: "Cliente",
  data: "",
  hora: "",
  enderecoPessoaAtendida: "",
}

type AgendamentoDialogProps = {
  profissionalId: string
  open: boolean
  onOpenChange: (aberto: boolean) => void
  onSalvo: () => void
}

export function AgendamentoDialog({ profissionalId, open, onOpenChange, onSalvo }: AgendamentoDialogProps) {
  const { accessToken } = useAuth()
  const [servicos, setServicos] = useState<ServicoResumo[]>([])
  const [erro, setErro] = useState<string | null>(null)

  const tipoAtendimentoPorServico = useMemo(
    () => Object.fromEntries(servicos.map((servico) => [servico.servicoId, servico.tipoAtendimento])) as Record<string, TipoAtendimento>,
    [servicos],
  )
  const esquema = useMemo(() => criarEsquemaAgendamento(tipoAtendimentoPorServico), [tipoAtendimentoPorServico])
  const form = useForm<AgendamentoFormValores>({
    resolver: standardSchemaResolver(esquema),
    defaultValues: VALORES_PADRAO,
  })

  const servicoSelecionado = servicos.find((servico) => servico.servicoId === form.watch("servicoId"))
  const exigeEndereco = servicoSelecionado?.tipoAtendimento === "Domiciliar"

  useEffect(() => {
    if (!open || !accessToken) {
      return
    }
    setErro(null)
    form.reset(VALORES_PADRAO)
    void listarServicos(profissionalId, accessToken).then(setServicos)
  }, [open, accessToken, profissionalId, form])

  async function aoSubmeter(dados: AgendamentoFormValores) {
    if (!accessToken) {
      return
    }
    setErro(null)
    try {
      await criarAgendamento(profissionalId, dados, accessToken)
      onOpenChange(false)
      onSalvo()
    } catch (erroCapturado) {
      setErro(traduzirErroDeAgendamento(erroCapturado))
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Novo agendamento</DialogTitle>
          <DialogDescription>Escolha o serviço, a pessoa atendida e o horário do atendimento.</DialogDescription>
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
              name="servicoId"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Serviço</FormLabel>
                  <Select value={field.value} onValueChange={field.onChange}>
                    <FormControl>
                      <SelectTrigger className="w-full">
                        <SelectValue placeholder="Selecione o serviço" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {servicos.map((servico) => (
                        <SelectItem key={servico.servicoId} value={servico.servicoId}>
                          {servico.nome}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="nomePessoaAtendida"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Nome da pessoa atendida</FormLabel>
                  <FormControl>
                    <Input placeholder="Maria Silva" {...field} />
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
                      <Input placeholder="Rua das Flores, 123" {...field} />
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
                {form.formState.isSubmitting ? "Agendando..." : "Agendar"}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  )
}
