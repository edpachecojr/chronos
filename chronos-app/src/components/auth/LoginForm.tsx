import { standardSchemaResolver } from "@hookform/resolvers/standard-schema"
import { CircleAlert } from "lucide-react"
import { useState } from "react"
import { useForm } from "react-hook-form"
import { Link } from "react-router-dom"

import { traduzirErroDeLogin } from "@/api/auth"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { Button } from "@/components/ui/button"
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form"
import { Input } from "@/components/ui/input"
import { PasswordInput } from "@/components/ui/password-input"
import { useAuth } from "@/hooks/useAuth"
import { esquemaLogin, type LoginFormValores } from "@/lib/validation/authSchemas"

export function LoginForm() {
  const { entrar } = useAuth()
  const [erro, setErro] = useState<string | null>(null)
  const form = useForm<LoginFormValores>({
    resolver: standardSchemaResolver(esquemaLogin),
    defaultValues: { email: "", password: "" },
  })

  async function aoSubmeter(dados: LoginFormValores) {
    setErro(null)
    try {
      await entrar(dados)
    } catch (erroCapturado) {
      setErro(traduzirErroDeLogin(erroCapturado))
    }
  }

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(aoSubmeter)} className="grid gap-4" noValidate>
        {erro && (
          <Alert
            variant="destructive"
            className="animate-in fade-in-0 slide-in-from-top-1 duration-200"
          >
            <CircleAlert aria-hidden="true" />
            <AlertDescription>{erro}</AlertDescription>
          </Alert>
        )}
        <FormField
          control={form.control}
          name="email"
          render={({ field }) => (
            <FormItem>
              <FormLabel>E-mail</FormLabel>
              <FormControl>
                <Input type="email" autoComplete="email" placeholder="voce@exemplo.com" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="password"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Senha</FormLabel>
              <FormControl>
                <PasswordInput autoComplete="current-password" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <Button type="submit" disabled={form.formState.isSubmitting} className="w-full">
          {form.formState.isSubmitting ? "Entrando..." : "Entrar"}
        </Button>
        <p className="text-center text-sm text-muted-foreground">
          Ainda não tem conta?{" "}
          <Link to="/registro" className="font-medium text-primary hover:underline">
            Cadastre-se
          </Link>
        </p>
      </form>
    </Form>
  )
}
