import { useEffect, useRef, useState } from "react"

import { listarProfissionais } from "@/api/profissionais"
import { useAuth } from "@/hooks/useAuth"

type EstadoProfissionalAtual = {
  profissionalId: string | null
  carregando: boolean
  erro: string | null
}

/**
 * Resolve o único profissional da organização corrente. O MVP não tem caso de uso de convite/adição de
 * profissional (apenas o onboarding cria o "profissional inicial"), então toda tela que precisa de um
 * `profissionalId` usa este hook em vez de um seletor.
 */
export function useProfissionalAtual(): EstadoProfissionalAtual {
  const { accessToken } = useAuth()
  const [estado, setEstado] = useState<EstadoProfissionalAtual>({ profissionalId: null, carregando: true, erro: null })
  const cacheRef = useRef<{ token: string; profissionalId: string } | null>(null)

  useEffect(() => {
    if (!accessToken) {
      setEstado({ profissionalId: null, carregando: false, erro: null })
      return
    }

    if (cacheRef.current?.token === accessToken) {
      setEstado({ profissionalId: cacheRef.current.profissionalId, carregando: false, erro: null })
      return
    }

    void resolverProfissional(accessToken)
  }, [accessToken])

  async function resolverProfissional(token: string) {
    setEstado({ profissionalId: null, carregando: true, erro: null })
    try {
      const profissionais = await listarProfissionais(token)
      const profissional = profissionais[0]
      if (!profissional) {
        setEstado({ profissionalId: null, carregando: false, erro: "Nenhum profissional encontrado para esta organização." })
        return
      }

      cacheRef.current = { token, profissionalId: profissional.profissionalId }
      setEstado({ profissionalId: profissional.profissionalId, carregando: false, erro: null })
    } catch {
      setEstado({ profissionalId: null, carregando: false, erro: "Não foi possível carregar os dados do profissional." })
    }
  }

  return estado
}
