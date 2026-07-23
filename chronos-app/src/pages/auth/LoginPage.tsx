import { AuthLayout } from "@/components/auth/AuthLayout"
import { LoginForm } from "@/components/auth/LoginForm"

export function LoginPage() {
  return (
    <AuthLayout titulo="Entrar no Chronos" descricao="Acesse sua conta para gerenciar sua agenda.">
      <LoginForm />
    </AuthLayout>
  )
}
