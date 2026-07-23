import { AuthLayout } from "@/components/auth/AuthLayout"
import { RegisterForm } from "@/components/auth/RegisterForm"

export function RegisterPage() {
  return (
    <AuthLayout titulo="Registrar-se" descricao="Leva menos de um minuto para começar.">
      <RegisterForm />
    </AuthLayout>
  )
}
