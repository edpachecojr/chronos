import type { Meta, StoryObj } from "@storybook/react-vite"
import { standardSchemaResolver } from "@hookform/resolvers/standard-schema"
import { useForm } from "react-hook-form"
import { z } from "zod"

import { Button } from "./button"
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "./form"
import { Input } from "./input"

const esquema = z.object({
  email: z.string().min(1, "Informe seu e-mail.").email("Informe um e-mail válido."),
})

function FormularioExemplo() {
  const form = useForm<z.infer<typeof esquema>>({
    resolver: standardSchemaResolver(esquema),
    defaultValues: { email: "" },
    mode: "onSubmit",
  })

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(() => {})} className="grid w-72 gap-4">
        <FormField
          control={form.control}
          name="email"
          render={({ field }) => (
            <FormItem>
              <FormLabel>E-mail</FormLabel>
              <FormControl>
                <Input type="email" placeholder="voce@exemplo.com" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <Button type="submit">Continuar</Button>
      </form>
    </Form>
  )
}

const meta = {
  title: "Design System/Form",
  component: FormularioExemplo,
  parameters: { layout: "centered" },
} satisfies Meta<typeof FormularioExemplo>

export default meta
type Story = StoryObj<typeof meta>

export const Default: Story = {}
