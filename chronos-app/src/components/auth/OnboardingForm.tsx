import { standardSchemaResolver } from "@hookform/resolvers/standard-schema"
import { CircleAlert } from "lucide-react"
import { useState } from "react"
import { useForm } from "react-hook-form"

import { traduzirErroDeOnboarding } from "@/api/organizacoes"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { Button } from "@/components/ui/button"
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form"
import { Input } from "@/components/ui/input"
import { useAuth } from "@/hooks/useAuth"
import { esquemaOnboarding, type OnboardingFormValores } from "@/lib/validation/authSchemas"

export function OnboardingForm() {
  const { completarOnboarding } = useAuth()
  const [erro, setErro] = useState<string | null>(null)
  const form = useForm<OnboardingFormValores>({
    resolver: standardSchemaResolver(esquemaOnboarding),
    defaultValues: { nome: "", nomeProfissionalInicial: "" },
  })

  async function aoSubmeter(dados: OnboardingFormValores) {
    setErro(null)
    try {
      await completarOnboarding(dados)
    } catch (erroCapturado) {
      setErro(traduzirErroDeOnboarding(erroCapturado))
    }
  }

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(aoSubmeter)} className="grid gap-4" noValidate>
        {erro && (
          <Alert variant="destructive">
            <CircleAlert aria-hidden="true" />
            <AlertDescription>{erro}</AlertDescription>
          </Alert>
        )}
        <FormField
          control={form.control}
          name="nome"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Nome do seu negócio</FormLabel>
              <FormControl>
                <Input placeholder="Clínica Bem-Estar" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="nomeProfissionalInicial"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Seu nome</FormLabel>
              <FormControl>
                <Input placeholder="Dra. Ana Souza" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <Button type="submit" disabled={form.formState.isSubmitting} className="w-full">
          {form.formState.isSubmitting ? "Concluindo..." : "Concluir"}
        </Button>
      </form>
    </Form>
  )
}
