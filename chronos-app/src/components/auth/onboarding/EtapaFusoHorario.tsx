import { standardSchemaResolver } from "@hookform/resolvers/standard-schema"
import { CircleAlert } from "lucide-react"
import { useState } from "react"
import { useForm } from "react-hook-form"

import { configurarPerfilOperacional, traduzirErroDePerfilOperacional } from "@/api/organizacoes"
import { PerfilOperacionalCampos } from "@/components/organizacao/PerfilOperacionalCampos"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { Button } from "@/components/ui/button"
import { Form } from "@/components/ui/form"
import { useAuth } from "@/hooks/useAuth"
import {
  FUSO_HORARIO_PADRAO,
  esquemaPerfilOperacional,
  type PerfilOperacionalFormValores,
} from "@/lib/validation/perfilOperacionalSchemas"

/** Etapa 2 do onboarding: fuso horário (pré-preenchido com UTC-3) e endereço do prestador opcional. */
export function EtapaFusoHorario() {
  const { accessToken, refrescarOrganizacao } = useAuth()
  const [erro, setErro] = useState<string | null>(null)
  const form = useForm<PerfilOperacionalFormValores>({
    resolver: standardSchemaResolver(esquemaPerfilOperacional),
    defaultValues: { fusoHorario: FUSO_HORARIO_PADRAO, enderecoPrestador: "" },
  })

  async function aoSubmeter(dados: PerfilOperacionalFormValores) {
    if (!accessToken) {
      return
    }
    setErro(null)
    try {
      await configurarPerfilOperacional(
        { fusoHorario: dados.fusoHorario, enderecoPrestador: dados.enderecoPrestador?.trim() || null },
        accessToken,
      )
      await refrescarOrganizacao()
    } catch (erroCapturado) {
      setErro(traduzirErroDePerfilOperacional(erroCapturado))
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
        <PerfilOperacionalCampos control={form.control} />
        <Button type="submit" disabled={form.formState.isSubmitting} className="w-full">
          {form.formState.isSubmitting ? "Salvando..." : "Continuar"}
        </Button>
      </form>
    </Form>
  )
}
