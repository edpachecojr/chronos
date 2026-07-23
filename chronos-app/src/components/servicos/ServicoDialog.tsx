import { standardSchemaResolver } from "@hookform/resolvers/standard-schema"
import { CircleAlert } from "lucide-react"
import { useEffect, useState } from "react"
import { useForm } from "react-hook-form"

import { criarServico, atualizarServico, paraDuracaoEmMinutos, traduzirErroDeServico, type ServicoResumo } from "@/api/servicos"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { Button } from "@/components/ui/button"
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog"
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form"
import { Input } from "@/components/ui/input"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { useAuth } from "@/hooks/useAuth"
import { esquemaServico, type ServicoFormValores } from "@/lib/validation/servicoSchemas"

const ROTULOS_TIPO_ATENDIMENTO: Record<ServicoFormValores["tipoAtendimento"], string> = {
  Online: "Online",
  Domiciliar: "Domiciliar",
  NoEnderecoDoPrestador: "No endereço do prestador",
}

const VALORES_PADRAO: ServicoFormValores = {
  nome: "",
  duracaoEmMinutos: 30,
  preco: 0,
  tipoAtendimento: "Online",
}

type ServicoDialogProps = {
  profissionalId: string
  servicoParaEditar?: ServicoResumo | null
  open: boolean
  onOpenChange: (aberto: boolean) => void
  onSalvo: () => void
}

export function ServicoDialog({ profissionalId, servicoParaEditar, open, onOpenChange, onSalvo }: ServicoDialogProps) {
  const { accessToken } = useAuth()
  const [erro, setErro] = useState<string | null>(null)
  const form = useForm<ServicoFormValores>({
    resolver: standardSchemaResolver(esquemaServico),
    defaultValues: VALORES_PADRAO,
  })

  useEffect(() => {
    if (!open) {
      return
    }
    setErro(null)
    form.reset(
      servicoParaEditar
        ? {
            nome: servicoParaEditar.nome,
            duracaoEmMinutos: paraDuracaoEmMinutos(servicoParaEditar.duracao),
            preco: servicoParaEditar.preco,
            tipoAtendimento: servicoParaEditar.tipoAtendimento,
          }
        : VALORES_PADRAO,
    )
  }, [open, servicoParaEditar, form])

  async function aoSubmeter(dados: ServicoFormValores) {
    if (!accessToken) {
      return
    }
    setErro(null)
    try {
      if (servicoParaEditar) {
        await atualizarServico(servicoParaEditar.servicoId, dados, accessToken)
      } else {
        await criarServico(profissionalId, dados, accessToken)
      }
      onOpenChange(false)
      onSalvo()
    } catch (erroCapturado) {
      setErro(traduzirErroDeServico(erroCapturado))
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{servicoParaEditar ? "Editar serviço" : "Novo serviço"}</DialogTitle>
          <DialogDescription>
            {servicoParaEditar
              ? "Atualize a configuração comercial deste serviço."
              : "Cadastre um serviço no catálogo do profissional."}
          </DialogDescription>
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
              name="nome"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Nome do serviço</FormLabel>
                  <FormControl>
                    <Input placeholder="Consulta inicial" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <div className="grid grid-cols-2 gap-4">
              <FormField
                control={form.control}
                name="duracaoEmMinutos"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Duração (minutos)</FormLabel>
                    <FormControl>
                      <Input
                        type="number"
                        min={1}
                        max={720}
                        {...field}
                        onChange={(evento) => field.onChange(evento.target.valueAsNumber)}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="preco"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Preço (R$)</FormLabel>
                    <FormControl>
                      <Input
                        type="number"
                        min={0}
                        step="0.01"
                        {...field}
                        onChange={(evento) => field.onChange(evento.target.valueAsNumber)}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>
            <FormField
              control={form.control}
              name="tipoAtendimento"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Modalidade</FormLabel>
                  <Select value={field.value} onValueChange={field.onChange}>
                    <FormControl>
                      <SelectTrigger className="w-full">
                        <SelectValue placeholder="Selecione a modalidade" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {Object.entries(ROTULOS_TIPO_ATENDIMENTO).map(([valor, rotulo]) => (
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
