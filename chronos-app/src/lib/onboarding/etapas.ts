import type { OrganizacaoAtual } from "@/api/organizacoes"

export type EtapaOnboarding = "organizacao" | "fusoHorario" | "disponibilidade" | "servico" | "concluido"

/**
 * Deriva a etapa atual do onboarding a partir dos dados já persistidos (sem um campo de progresso à parte),
 * para o wizard sempre retomar no ponto certo, inclusive após recarregar a página.
 */
export function resolverEtapaOnboarding(organizacao: OrganizacaoAtual | null): EtapaOnboarding {
  if (!organizacao) {
    return "organizacao"
  }
  if (!organizacao.fusoHorario) {
    return "fusoHorario"
  }
  if (!organizacao.possuiDisponibilidade) {
    return "disponibilidade"
  }
  if (!organizacao.possuiServico) {
    return "servico"
  }
  return "concluido"
}
