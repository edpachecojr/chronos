import { OnboardingForm } from "@/components/auth/OnboardingForm"
import { EtapaDisponibilidade } from "@/components/auth/onboarding/EtapaDisponibilidade"
import { EtapaFusoHorario } from "@/components/auth/onboarding/EtapaFusoHorario"
import { EtapaServico } from "@/components/auth/onboarding/EtapaServico"
import { OnboardingProgresso } from "@/components/auth/onboarding/OnboardingProgresso"
import { useAuth } from "@/hooks/useAuth"
import { resolverEtapaOnboarding } from "@/lib/onboarding/etapas"

/**
 * Wizard sequencial de onboarding: organização → fuso horário → disponibilidade → primeiro serviço. A etapa atual é
 * derivada dos dados já persistidos (via `resolverEtapaOnboarding`), não de um estado local — por isso o wizard
 * sempre retoma na etapa certa, inclusive após recarregar a página.
 */
export function OnboardingWizard() {
  const { organizacao } = useAuth()
  const etapa = resolverEtapaOnboarding(organizacao)

  if (etapa === "organizacao") {
    return <OnboardingForm />
  }

  if (etapa === "concluido") {
    return null
  }

  return (
    <div className="grid gap-6">
      <OnboardingProgresso etapaAtual={etapa} />
      {etapa === "fusoHorario" && <EtapaFusoHorario />}
      {etapa === "disponibilidade" && <EtapaDisponibilidade />}
      {etapa === "servico" && <EtapaServico />}
    </div>
  )
}
