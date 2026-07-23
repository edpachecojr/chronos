import { AuthLayout } from "@/components/auth/AuthLayout"
import { OnboardingForm } from "@/components/auth/OnboardingForm"

export function OnboardingPage() {
  return (
    <AuthLayout titulo="Configure seu negócio" descricao="Só mais um passo antes de acessar sua agenda.">
      <OnboardingForm />
    </AuthLayout>
  )
}
