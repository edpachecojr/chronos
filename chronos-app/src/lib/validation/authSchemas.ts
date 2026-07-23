import { z } from "zod"

/** Espelha a política de senha padrão do ASP.NET Core Identity (`AdicionarIdentity`, sem overrides). */
export const esquemaSenha = z
  .string()
  .min(6, "A senha deve ter pelo menos 6 caracteres.")
  .regex(/[0-9]/, "A senha deve conter pelo menos um número.")
  .regex(/[a-z]/, "A senha deve conter pelo menos uma letra minúscula.")
  .regex(/[A-Z]/, "A senha deve conter pelo menos uma letra maiúscula.")
  .regex(/[^a-zA-Z0-9]/, "A senha deve conter pelo menos um caractere especial.")

export const esquemaCadastro = z
  .object({
    email: z.string().min(1, "Informe seu e-mail.").email("Informe um e-mail válido."),
    password: esquemaSenha,
    confirmarSenha: z.string().min(1, "Confirme sua senha."),
    aceitaTermos: z.boolean().refine((valor) => valor === true, {
      message: "É necessário aceitar os termos de uso.",
    }),
  })
  .refine((dados) => dados.password === dados.confirmarSenha, {
    message: "As senhas não coincidem.",
    path: ["confirmarSenha"],
  })

export type CadastroFormValores = z.infer<typeof esquemaCadastro>

export const esquemaLogin = z.object({
  email: z.string().min(1, "Informe seu e-mail.").email("Informe um e-mail válido."),
  password: z.string().min(1, "Informe sua senha."),
})

export type LoginFormValores = z.infer<typeof esquemaLogin>

export const esquemaOnboarding = z.object({
  nome: z.string().min(1, "Informe o nome do seu negócio.").max(120, "O nome deve ter até 120 caracteres."),
  nomeProfissionalInicial: z
    .string()
    .min(1, "Informe seu nome.")
    .max(120, "O nome deve ter até 120 caracteres."),
})

export type OnboardingFormValores = z.infer<typeof esquemaOnboarding>
