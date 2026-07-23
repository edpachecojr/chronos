import { z } from "zod"

export const FUSO_HORARIO_PADRAO = "America/Sao_Paulo"

export const esquemaPerfilOperacional = z.object({
  fusoHorario: z.string().min(1, "Selecione o fuso horário."),
  enderecoPrestador: z.string().max(300, "O endereço deve ter até 300 caracteres.").optional(),
})

export type PerfilOperacionalFormValores = z.infer<typeof esquemaPerfilOperacional>
