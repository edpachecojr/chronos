import { z } from "zod"

const DIAS_DA_SEMANA = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"] as const

export const esquemaDisponibilidade = z
  .object({
    diaDaSemana: z.enum(DIAS_DA_SEMANA, { error: "Selecione o dia da semana." }),
    inicio: z.string().min(1, "Informe o horário de início."),
    fim: z.string().min(1, "Informe o horário de fim."),
  })
  .refine((dados) => dados.fim > dados.inicio, {
    message: "O horário de fim deve ser depois do início.",
    path: ["fim"],
  })

export type DisponibilidadeFormValores = z.infer<typeof esquemaDisponibilidade>
