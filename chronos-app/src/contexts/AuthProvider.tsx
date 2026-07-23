import { useEffect, useState, type ReactNode } from "react"

import { autenticar, registrarUsuario, type LoginEntrada, type RegistrarUsuarioEntrada } from "@/api/auth"
import { ErroApi } from "@/api/http"
import {
  consultarOrganizacaoAtual,
  onboardOrganizacao,
  type OnboardOrganizacaoEntrada,
  type OrganizacaoAtual,
} from "@/api/organizacoes"
import { gravarSessaoArmazenada, lerSessaoArmazenada, limparSessaoArmazenada } from "@/api/tokenStorage"
import { AuthContext, type StatusSessao } from "@/contexts/AuthContext"
import { resolverEtapaOnboarding } from "@/lib/onboarding/etapas"

/** Provê a sessão do usuário autenticado (tokens, organização) a toda a árvore, persistindo-a em `localStorage`. */
export function AuthProvider({ children }: { children: ReactNode }) {
  const [status, setStatus] = useState<StatusSessao>("carregando")
  const [organizacao, setOrganizacao] = useState<OrganizacaoAtual | null>(null)
  const [accessToken, setAccessToken] = useState<string | null>(null)
  const [profissionalId, setProfissionalId] = useState<string | null>(null)

  useEffect(() => {
    const sessao = lerSessaoArmazenada()
    if (sessao) {
      void sincronizarOrganizacao(sessao.accessToken)
    } else {
      setStatus("nao_autenticado")
    }
  }, [])

  async function sincronizarOrganizacao(token: string) {
    setAccessToken(token)
    try {
      const org = await consultarOrganizacaoAtual(token)
      setOrganizacao(org)
      const etapa = resolverEtapaOnboarding(org)
      setStatus(etapa === "concluido" ? "autenticado_onboarding_concluido" : "autenticado_onboarding_pendente")
    } catch (erro) {
      if (erro instanceof ErroApi && erro.status === 401) {
        sair()
        return
      }
      throw erro
    }
  }

  async function refrescarOrganizacao() {
    if (!accessToken) {
      return
    }
    await sincronizarOrganizacao(accessToken)
  }

  async function entrar(dados: LoginEntrada) {
    const token = await autenticar(dados)
    gravarSessaoArmazenada({ accessToken: token.accessToken, refreshToken: token.refreshToken })
    await sincronizarOrganizacao(token.accessToken)
  }

  async function registrar(dados: RegistrarUsuarioEntrada) {
    await registrarUsuario(dados)
    await entrar(dados)
  }

  async function completarOnboarding(dados: OnboardOrganizacaoEntrada) {
    if (!accessToken) {
      throw new Error("É necessário estar autenticado para concluir o onboarding.")
    }
    const resultado = await onboardOrganizacao(dados, accessToken)
    setProfissionalId(resultado.profissionalId)
    await sincronizarOrganizacao(accessToken)
  }

  function sair() {
    limparSessaoArmazenada()
    setAccessToken(null)
    setOrganizacao(null)
    setProfissionalId(null)
    setStatus("nao_autenticado")
  }

  return (
    <AuthContext.Provider
      value={{
        status,
        organizacao,
        accessToken,
        profissionalId,
        entrar,
        registrar,
        completarOnboarding,
        refrescarOrganizacao,
        sair,
      }}
    >
      {children}
    </AuthContext.Provider>
  )
}
