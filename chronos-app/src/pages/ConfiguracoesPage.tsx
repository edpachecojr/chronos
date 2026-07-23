import { standardSchemaResolver } from "@hookform/resolvers/standard-schema"
import { useForm } from "react-hook-form"
import { toast } from "sonner"

import { configurarPerfilOperacional, traduzirErroDePerfilOperacional, type OrganizacaoAtual } from "@/api/organizacoes"
import { EstadoCarregando } from "@/components/estado/EstadoCarregando"
import { PerfilOperacionalCampos } from "@/components/organizacao/PerfilOperacionalCampos"
import { Button } from "@/components/ui/button"
import { Form } from "@/components/ui/form"
import { useAuth } from "@/hooks/useAuth"
import {
  FUSO_HORARIO_PADRAO,
  esquemaPerfilOperacional,
  type PerfilOperacionalFormValores,
} from "@/lib/validation/perfilOperacionalSchemas"

export function ConfiguracoesPage() {
  const { organizacao } = useAuth()

  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col gap-1">
        <h1 className="text-2xl font-semibold tracking-tight text-foreground">Configurações</h1>
        <p className="text-sm text-muted-foreground">
          Endereço do prestador e fuso horário usados para calcular local e disponibilidade dos agendamentos.
        </p>
      </div>
      {organizacao ? <PerfilOperacionalForm organizacao={organizacao} /> : <EstadoCarregando />}
    </div>
  )
}

type PerfilOperacionalFormProps = {
  organizacao: OrganizacaoAtual
}

/** Formulário isolado num componente próprio para que `defaultValues` já nasça com os dados atuais da organização (sem reset/sync posterior). */
function PerfilOperacionalForm({ organizacao }: PerfilOperacionalFormProps) {
  const { accessToken, refrescarOrganizacao } = useAuth()
  const form = useForm<PerfilOperacionalFormValores>({
    resolver: standardSchemaResolver(esquemaPerfilOperacional),
    defaultValues: {
      fusoHorario: organizacao.fusoHorario ?? FUSO_HORARIO_PADRAO,
      enderecoPrestador: organizacao.enderecoPrestador ?? "",
    },
  })

  async function aoSubmeter(dados: PerfilOperacionalFormValores) {
    if (!accessToken) {
      return
    }
    try {
      await configurarPerfilOperacional(
        { fusoHorario: dados.fusoHorario, enderecoPrestador: dados.enderecoPrestador?.trim() || null },
        accessToken,
      )
      await refrescarOrganizacao()
      toast.success("Perfil operacional atualizado.")
    } catch (erroCapturado) {
      toast.error(traduzirErroDePerfilOperacional(erroCapturado))
    }
  }

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(aoSubmeter)} className="grid max-w-sm gap-4" noValidate>
        <PerfilOperacionalCampos control={form.control} />
        <Button type="submit" disabled={form.formState.isSubmitting} className="w-fit">
          {form.formState.isSubmitting ? "Salvando..." : "Salvar"}
        </Button>
      </form>
    </Form>
  )
}
