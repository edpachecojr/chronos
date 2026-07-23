import { AuthLayout } from "@/components/auth/AuthLayout"
import { RegisterForm } from "@/components/auth/RegisterForm"

export function RegisterPage() {
  return (
    <AuthLayout titulo="Criar conta no Chronos" descricao="Leva menos de um minuto para começar.">
      <RegisterForm />
    </AuthLayout>
  )
}
