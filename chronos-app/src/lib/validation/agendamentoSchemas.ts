import { z } from "zod"

import type { TipoAtendimento } from "@/api/servicos"

const TIPOS_PESSOA_ATENDIDA = ["Cliente", "Paciente", "Aluno", "Outro"] as const

const CAMPOS_BASE = {
  servicoId: z.string().min(1, "Selecione o serviço."),
  nomePessoaAtendida: z.string().min(1, "Informe o nome da pessoa atendida.").max(120, "O nome deve ter até 120 caracteres."),
  tipoPessoaAtendida: z.enum(TIPOS_PESSOA_ATENDIDA, { error: "Selecione o tipo de pessoa atendida." }),
  data: z.string().min(1, "Informe a data."),
  hora: z.string().min(1, "Informe o horário."),
  enderecoPessoaAtendida: z.string().max(300, "O endereço deve ter até 300 caracteres."),
}

/**
 * Monta o schema de criação de agendamento (UC04): o endereço da pessoa atendida só é obrigatório quando o
 * serviço escolhido é domiciliar (RN06), por isso o schema depende do catálogo de serviços carregado.
 */
export function criarEsquemaAgendamento(tipoAtendimentoPorServico: Record<string, TipoAtendimento>) {
  return z.object(CAMPOS_BASE).superRefine((dados, contexto) => {
    const exigeEndereco = tipoAtendimentoPorServico[dados.servicoId] === "Domiciliar"
    if (exigeEndereco && dados.enderecoPessoaAtendida.trim().length === 0) {
      contexto.addIssue({
        code: "custom",
        message: "O atendimento domiciliar exige o endereço da pessoa atendida.",
        path: ["enderecoPessoaAtendida"],
      })
    }
  })
}

export type AgendamentoFormValores = z.infer<ReturnType<typeof criarEsquemaAgendamento>>

const CAMPOS_REAGENDAMENTO = {
  nomePessoaAtendida: CAMPOS_BASE.nomePessoaAtendida,
  tipoPessoaAtendida: CAMPOS_BASE.tipoPessoaAtendida,
  data: CAMPOS_BASE.data,
  hora: CAMPOS_BASE.hora,
  enderecoPessoaAtendida: CAMPOS_BASE.enderecoPessoaAtendida,
}

/**
 * Monta o schema de reagendamento (UC05): sem `servicoId`/`profissionalId`, imutáveis após a criação. `exigeEndereco`
 * já vem resolvido pelo chamador, que conhece o serviço fixo deste agendamento.
 */
export function criarEsquemaReagendamento(exigeEndereco: boolean) {
  return z.object(CAMPOS_REAGENDAMENTO).superRefine((dados, contexto) => {
    if (exigeEndereco && dados.enderecoPessoaAtendida.trim().length === 0) {
      contexto.addIssue({
        code: "custom",
        message: "O atendimento domiciliar exige o endereço da pessoa atendida.",
        path: ["enderecoPessoaAtendida"],
      })
    }
  })
}

export type ReagendamentoFormValores = z.infer<ReturnType<typeof criarEsquemaReagendamento>>
