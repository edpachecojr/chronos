import type { Control } from "react-hook-form"

import { FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form"
import { Input } from "@/components/ui/input"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import type { ServicoFormValores } from "@/lib/validation/servicoSchemas"

const ROTULOS_TIPO_ATENDIMENTO: Record<ServicoFormValores["tipoAtendimento"], string> = {
  Online: "Online",
  Domiciliar: "Domiciliar",
  NoEnderecoDoPrestador: "No endereço do prestador",
}

type ServicoCamposProps = {
  control: Control<ServicoFormValores>
}

/** Campos de nome, duração, preço e modalidade, reaproveitados pelo dialog de serviço (UC03) e pela etapa de serviço do onboarding. */
export function ServicoCampos({ control }: ServicoCamposProps) {
  return (
    <>
      <FormField
        control={control}
        name="nome"
        render={({ field }) => (
          <FormItem>
            <FormLabel>Nome do serviço</FormLabel>
            <FormControl>
              <Input placeholder="Consulta inicial" {...field} />
            </FormControl>
            <FormMessage />
          </FormItem>
        )}
      />
      <div className="grid grid-cols-2 gap-4">
        <FormField
          control={control}
          name="duracaoEmMinutos"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Duração (minutos)</FormLabel>
              <FormControl>
                <Input
                  type="number"
                  min={1}
                  max={720}
                  {...field}
                  onChange={(evento) => field.onChange(evento.target.valueAsNumber)}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={control}
          name="preco"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Preço (R$)</FormLabel>
              <FormControl>
                <Input
                  type="number"
                  min={0}
                  step="0.01"
                  {...field}
                  onChange={(evento) => field.onChange(evento.target.valueAsNumber)}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      </div>
      <FormField
        control={control}
        name="tipoAtendimento"
        render={({ field }) => (
          <FormItem>
            <FormLabel>Modalidade</FormLabel>
            <Select value={field.value} onValueChange={field.onChange}>
              <FormControl>
                <SelectTrigger className="w-full">
                  <SelectValue placeholder="Selecione a modalidade" />
                </SelectTrigger>
              </FormControl>
              <SelectContent>
                {Object.entries(ROTULOS_TIPO_ATENDIMENTO).map(([valor, rotulo]) => (
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
    </>
  )
}
