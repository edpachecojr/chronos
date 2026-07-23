import type { ReactNode } from "react"
import { Navigate } from "react-router-dom"

import { CarregandoSessao } from "@/components/auth/CarregandoSessao"
import { useAuth } from "@/hooks/useAuth"

/** Exige uma sessão autenticada e com onboarding (UC01) concluído. */
export function ProtectedRoute({ children }: { children: ReactNode }) {
  const { status } = useAuth()

  if (status === "carregando") {
    return <CarregandoSessao />
  }
  if (status === "nao_autenticado") {
    return <Navigate to="/login" replace />
  }
  if (status === "autenticado_sem_organizacao") {
    return <Navigate to="/onboarding" replace />
  }

  return <>{children}</>
}
