import { z } from "zod"

export const esquemaServico = z.object({
  nome: z.string().min(1, "Informe o nome do serviço.").max(120, "O nome deve ter até 120 caracteres."),
  duracaoEmMinutos: z
    .number({ error: "Informe a duração em minutos." })
    .int("A duração deve ser um número inteiro de minutos.")
    .positive("A duração deve ser maior que zero.")
    .max(720, "A duração deve ser de até 12 horas (720 minutos)."),
  preco: z
    .number({ error: "Informe o preço do serviço." })
    .min(0, "O preço não pode ser negativo.")
    .refine(
      (valor) => Math.abs(valor * 100 - Math.round(valor * 100)) < 1e-9,
      "O preço deve ter até duas casas decimais.",
    ),
  tipoAtendimento: z.enum(["Online", "Domiciliar", "NoEnderecoDoPrestador"], {
    error: "Selecione a modalidade de atendimento.",
  }),
})

export type ServicoFormValores = z.infer<typeof esquemaServico>
