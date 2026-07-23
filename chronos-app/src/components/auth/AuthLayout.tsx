import type { ReactNode } from "react"

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"

type AuthLayoutProps = {
  titulo: string
  descricao: string
  children: ReactNode
}

export function AuthLayout({ titulo, descricao, children }: AuthLayoutProps) {
  return (
    <main className="flex min-h-svh items-center justify-center bg-background p-6">
      <Card className="w-full max-w-sm">
        <CardHeader>
          <CardTitle>{titulo}</CardTitle>
          <CardDescription>{descricao}</CardDescription>
        </CardHeader>
        <CardContent>{children}</CardContent>
      </Card>
    </main>
  )
}
