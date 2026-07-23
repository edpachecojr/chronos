import { createContext } from "react"

import type { LoginEntrada, RegistrarUsuarioEntrada } from "@/api/auth"
import type { OnboardOrganizacaoEntrada, OrganizacaoAtual } from "@/api/organizacoes"

export type StatusSessao =
  | "carregando"
  | "nao_autenticado"
  | "autenticado_sem_organizacao"
  | "autenticado_com_organizacao"

export type AuthContextValue = {
  status: StatusSessao
  organizacao: OrganizacaoAtual | null
  entrar: (dados: LoginEntrada) => Promise<void>
  registrar: (dados: RegistrarUsuarioEntrada) => Promise<void>
  completarOnboarding: (dados: OnboardOrganizacaoEntrada) => Promise<void>
  sair: () => void
}

export const AuthContext = createContext<AuthContextValue | null>(null)
