import { standardSchemaResolver } from "@hookform/resolvers/standard-schema"
import { CircleAlert } from "lucide-react"
import { useCallback, useEffect, useState } from "react"
import { useForm } from "react-hook-form"

import {
  ROTULOS_DIA_DA_SEMANA,
  criarDisponibilidade,
  listarDisponibilidades,
  traduzirErroDeDisponibilidade,
  type DisponibilidadeResumo,
} from "@/api/disponibilidades"
import { DisponibilidadeCampos } from "@/components/disponibilidade/DisponibilidadeCampos"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Form } from "@/components/ui/form"
import { Separator } from "@/components/ui/separator"
import { useAuth } from "@/hooks/useAuth"
import { useProfissionalAtual } from "@/hooks/useProfissionalAtual"
import { esquemaDisponibilidade, type DisponibilidadeFormValores } from "@/lib/validation/disponibilidadeSchemas"

const VALORES_PADRAO: DisponibilidadeFormValores = { diaDaSemana: "Monday", inicio: "09:00", fim: "18:00" }

/** Etapa 3 do onboarding: ao menos uma janela de disponibilidade semanal, exigida para criar agendamentos (RN07). */
export function EtapaDisponibilidade() {
  const { accessToken, refrescarOrganizacao } = useAuth()
  const { profissionalId } = useProfissionalAtual()
  const [disponibilidades, setDisponibilidades] = useState<DisponibilidadeResumo[] | null>(null)
  const [erro, setErro] = useState<string | null>(null)
  const [continuando, setContinuando] = useState(false)
  const form = useForm<DisponibilidadeFormValores>({
    resolver: standardSchemaResolver(esquemaDisponibilidade),
    defaultValues: VALORES_PADRAO,
  })

  const carregarDisponibilidades = useCallback(async () => {
    if (!accessToken || !profissionalId) {
      return
    }
    const resultado = await listarDisponibilidades(profissionalId, accessToken)
    setDisponibilidades(resultado)
  }, [accessToken, profissionalId])

  useEffect(() => {
    void carregarDisponibilidades()
  }, [carregarDisponibilidades])

  async function aoAdicionar(dados: DisponibilidadeFormValores) {
    if (!accessToken || !profissionalId) {
      return
    }
    setErro(null)
    try {
      await criarDisponibilidade(profissionalId, dados, accessToken)
      form.reset(VALORES_PADRAO)
      await carregarDisponibilidades()
    } catch (erroCapturado) {
      setErro(traduzirErroDeDisponibilidade(erroCapturado))
    }
  }

  async function aoContinuar() {
    setContinuando(true)
    try {
      await refrescarOrganizacao()
    } finally {
      setContinuando(false)
    }
  }

  return (
    <div className="grid gap-4">
      {disponibilidades && disponibilidades.length > 0 && (
        <>
          <ul className="flex flex-col gap-2">
            {disponibilidades.map((disponibilidade) => (
              <li key={disponibilidade.disponibilidadeId} className="flex items-center gap-2 text-sm">
                <Badge variant="secondary">{ROTULOS_DIA_DA_SEMANA[disponibilidade.diaDaSemana]}</Badge>
                <span className="font-medium text-foreground">
                  {disponibilidade.inicio.slice(0, 5)}–{disponibilidade.fim.slice(0, 5)}
                </span>
              </li>
            ))}
          </ul>
          <Separator />
        </>
      )}
      <Form {...form}>
        <form onSubmit={form.handleSubmit(aoAdicionar)} className="grid gap-4" noValidate>
          {erro && (
            <Alert variant="destructive" className="animate-in fade-in-0 slide-in-from-top-1 duration-200">
              <CircleAlert aria-hidden="true" />
              <AlertDescription>{erro}</AlertDescription>
            </Alert>
          )}
          <DisponibilidadeCampos control={form.control} />
          <Button type="submit" variant="outline" disabled={form.formState.isSubmitting} className="w-full">
            {form.formState.isSubmitting ? "Adicionando..." : "Adicionar disponibilidade"}
          </Button>
        </form>
      </Form>
      <Button
        type="button"
        onClick={aoContinuar}
        disabled={!disponibilidades || disponibilidades.length === 0 || continuando}
        className="w-full"
      >
        {continuando ? "Salvando..." : "Continuar"}
      </Button>
    </div>
  )
}
