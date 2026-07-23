import { standardSchemaResolver } from "@hookform/resolvers/standard-schema"
import { CircleAlert } from "lucide-react"
import { useEffect, useState } from "react"
import { useForm } from "react-hook-form"

import { criarServico, atualizarServico, paraDuracaoEmMinutos, traduzirErroDeServico, type ServicoResumo } from "@/api/servicos"
import { ServicoCampos } from "@/components/servicos/ServicoCampos"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { Button } from "@/components/ui/button"
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog"
import { Form } from "@/components/ui/form"
import { useAuth } from "@/hooks/useAuth"
import { esquemaServico, type ServicoFormValores } from "@/lib/validation/servicoSchemas"

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
            <ServicoCampos control={form.control} />
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
