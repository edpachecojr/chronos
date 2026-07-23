import type { ReactNode } from "react"

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"

type EstadoVazioProps = {
  titulo: string
  descricao?: string
  acao?: ReactNode
}

export function EstadoVazio({ titulo, descricao, acao }: EstadoVazioProps) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>{titulo}</CardTitle>
        {descricao && <CardDescription>{descricao}</CardDescription>}
      </CardHeader>
      {acao && <CardContent>{acao}</CardContent>}
    </Card>
  )
}
