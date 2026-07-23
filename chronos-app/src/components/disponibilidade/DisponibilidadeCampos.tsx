import type { Control } from "react-hook-form"

import { ROTULOS_DIA_DA_SEMANA } from "@/api/disponibilidades"
import { FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form"
import { Input } from "@/components/ui/input"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import type { DisponibilidadeFormValores } from "@/lib/validation/disponibilidadeSchemas"

type DisponibilidadeCamposProps = {
  control: Control<DisponibilidadeFormValores>
}

/** Campos de dia da semana e janela de horário, reaproveitados pelo dialog de disponibilidade (UC02) e pela etapa de disponibilidade do onboarding. */
export function DisponibilidadeCampos({ control }: DisponibilidadeCamposProps) {
  return (
    <>
      <FormField
        control={control}
        name="diaDaSemana"
        render={({ field }) => (
          <FormItem>
            <FormLabel>Dia da semana</FormLabel>
            <Select value={field.value} onValueChange={field.onChange}>
              <FormControl>
                <SelectTrigger className="w-full">
                  <SelectValue placeholder="Selecione o dia" />
                </SelectTrigger>
              </FormControl>
              <SelectContent>
                {Object.entries(ROTULOS_DIA_DA_SEMANA).map(([valor, rotulo]) => (
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
      <div className="grid grid-cols-2 gap-4">
        <FormField
          control={control}
          name="inicio"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Início</FormLabel>
              <FormControl>
                <Input type="time" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={control}
          name="fim"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Fim</FormLabel>
              <FormControl>
                <Input type="time" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      </div>
    </>
  )
}
