import type { Control } from "react-hook-form"

import { FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form"
import { Input } from "@/components/ui/input"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import type { PerfilOperacionalFormValores } from "@/lib/validation/perfilOperacionalSchemas"

const ROTULOS_FUSO_HORARIO: Record<string, string> = {
  "America/Noronha": "Fernando de Noronha (UTC-2)",
  "America/Sao_Paulo": "Horário de Brasília (UTC-3)",
  "America/Manaus": "Amazonas (UTC-4)",
  "America/Rio_Branco": "Acre (UTC-5)",
}

type PerfilOperacionalCamposProps = {
  control: Control<PerfilOperacionalFormValores>
}

/** Campos de fuso horário e endereço do prestador, reaproveitados pela etapa de fuso horário do onboarding e pela ConfiguracoesPage. */
export function PerfilOperacionalCampos({ control }: PerfilOperacionalCamposProps) {
  return (
    <>
      <FormField
        control={control}
        name="fusoHorario"
        render={({ field }) => (
          <FormItem>
            <FormLabel>Fuso horário</FormLabel>
            <Select value={field.value} onValueChange={field.onChange}>
              <FormControl>
                <SelectTrigger className="w-full">
                  <SelectValue placeholder="Selecione o fuso horário" />
                </SelectTrigger>
              </FormControl>
              <SelectContent>
                {Object.entries(ROTULOS_FUSO_HORARIO).map(([valor, rotulo]) => (
                  <SelectItem key={valor} value={valor}>
                    {rotulo}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            <FormMessage />
          </FormItem>
        )}
      />
      <FormField
        control={control}
        name="enderecoPrestador"
        render={({ field }) => (
          <FormItem>
            <FormLabel>Endereço do prestador (opcional)</FormLabel>
            <FormControl>
              <Input placeholder="Av. Central, 20" {...field} />
            </FormControl>
            <FormMessage />
          </FormItem>
        )}
      />
    </>
  )
}
