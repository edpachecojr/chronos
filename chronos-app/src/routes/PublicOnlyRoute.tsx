import type { ReactNode } from "react"
import { Navigate } from "react-router-dom"

import { CarregandoSessao } from "@/components/auth/CarregandoSessao"
import { useAuth } from "@/hooks/useAuth"

/** Usado por telas de login/cadastro: afasta um usuário já autenticado. */
export function PublicOnlyRoute({ children }: { children: ReactNode }) {
  const { status } = useAuth()

  if (status === "carregando") {
    return <CarregandoSessao />
  }
  if (status === "autenticado_onboarding_pendente") {
    return <Navigate to="/onboarding" replace />
  }
  if (status === "autenticado_onboarding_concluido") {
    return <Navigate to="/" replace />
  }

  return <>{children}</>
}
