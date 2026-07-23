import type { ReactNode } from "react"
import { Navigate } from "react-router-dom"

import { CarregandoSessao } from "@/components/auth/CarregandoSessao"
import { useAuth } from "@/hooks/useAuth"

/** Só é acessível entre o login e a conclusão do onboarding (UC01). */
export function OnboardingRoute({ children }: { children: ReactNode }) {
  const { status } = useAuth()

  if (status === "carregando") {
    return <CarregandoSessao />
  }
  if (status === "nao_autenticado") {
    return <Navigate to="/login" replace />
  }
  if (status === "autenticado_onboarding_concluido") {
    return <Navigate to="/" replace />
  }

  return <>{children}</>
}
