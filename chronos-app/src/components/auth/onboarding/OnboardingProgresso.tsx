import { Progress } from "@/components/ui/progress"
import type { EtapaOnboarding } from "@/lib/onboarding/etapas"

const ETAPAS: { etapa: EtapaOnboarding; rotulo: string }[] = [
  { etapa: "organizacao", rotulo: "Organização" },
  { etapa: "fusoHorario", rotulo: "Fuso horário" },
  { etapa: "disponibilidade", rotulo: "Disponibilidade" },
  { etapa: "servico", rotulo: "Primeiro serviço" },
]

type OnboardingProgressoProps = {
  etapaAtual: EtapaOnboarding
}

/** Indicador de progresso do wizard de onboarding: rótulo textual + barra, sem navegação livre entre etapas. */
export function OnboardingProgresso({ etapaAtual }: OnboardingProgressoProps) {
  const indice = ETAPAS.findIndex((item) => item.etapa === etapaAtual)
  const numeroEtapa = indice + 1
  const rotulo = ETAPAS[indice]?.rotulo ?? ""

  return (
    <div className="flex flex-col gap-2">
      <p className="text-sm font-medium text-muted-foreground">
        Etapa {numeroEtapa} de {ETAPAS.length} — {rotulo}
      </p>
      <Progress value={(numeroEtapa / ETAPAS.length) * 100} aria-label={`Progresso do onboarding: ${rotulo}`} />
    </div>
  )
}
