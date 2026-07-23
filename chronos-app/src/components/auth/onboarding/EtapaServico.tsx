import { standardSchemaResolver } from "@hookform/resolvers/standard-schema"
import { CircleAlert } from "lucide-react"
import { useState } from "react"
import { useForm } from "react-hook-form"

import { criarServico, traduzirErroDeServico } from "@/api/servicos"
import { ServicoCampos } from "@/components/servicos/ServicoCampos"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { Button } from "@/components/ui/button"
import { Form } from "@/components/ui/form"
import { useAuth } from "@/hooks/useAuth"
import { useProfissionalAtual } from "@/hooks/useProfissionalAtual"
import { esquemaServico, type ServicoFormValores } from "@/lib/validation/servicoSchemas"

const VALORES_PADRAO: ServicoFormValores = { nome: "", duracaoEmMinutos: 30, preco: 0, tipoAtendimento: "Online" }

/** Etapa 4 (final) do onboarding: primeiro serviço do catálogo, exigido para criar agendamentos. */
export function EtapaServico() {
  const { accessToken, refrescarOrganizacao } = useAuth()
  const { profissionalId } = useProfissionalAtual()
  const [erro, setErro] = useState<string | null>(null)
  const form = useForm<ServicoFormValores>({
    resolver: standardSchemaResolver(esquemaServico),
    defaultValues: VALORES_PADRAO,
  })

  async function aoSubmeter(dados: ServicoFormValores) {
    if (!accessToken || !profissionalId) {
      return
    }
    setErro(null)
    try {
      await criarServico(profissionalId, dados, accessToken)
      await refrescarOrganizacao()
    } catch (erroCapturado) {
      setErro(traduzirErroDeServico(erroCapturado))
    }
  }

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(aoSubmeter)} className="grid gap-4" noValidate>
        {erro && (
          <Alert variant="destructive" className="animate-in fade-in-0 slide-in-from-top-1 duration-200">
            <CircleAlert aria-hidden="true" />
            <AlertDescription>{erro}</AlertDescription>
          </Alert>
        )}
        <ServicoCampos control={form.control} />
        <Button type="submit" disabled={form.formState.isSubmitting} className="w-full">
          {form.formState.isSubmitting ? "Concluindo..." : "Concluir"}
        </Button>
      </form>
    </Form>
  )
}
