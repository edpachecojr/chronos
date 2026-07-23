import { AuthLayout } from "@/components/auth/AuthLayout"
import { OnboardingWizard } from "@/components/auth/onboarding/OnboardingWizard"

export function OnboardingPage() {
  return (
    <AuthLayout titulo="Configure seu negócio" descricao="Só mais alguns passos antes de acessar sua agenda.">
      <OnboardingWizard />
    </AuthLayout>
  )
}
