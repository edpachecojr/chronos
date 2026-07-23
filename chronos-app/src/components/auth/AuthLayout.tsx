import type { ReactNode } from "react"

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"

type AuthLayoutProps = {
  titulo: string
  descricao: string
  children: ReactNode
}

export function AuthLayout({ titulo, descricao, children }: AuthLayoutProps) {
  return (
    <main className="flex min-h-svh items-center justify-center bg-background p-4 sm:p-6">
      <Card className="w-full max-w-sm animate-in fade-in-0 slide-in-from-bottom-4 shadow-lg shadow-neutral-900/5 duration-300 ease-out">
        <CardHeader className="gap-2 text-center">
          <CardTitle className="text-2xl font-semibold tracking-tight text-balance">
            {titulo}
          </CardTitle>
          <CardDescription className="text-balance leading-relaxed">{descricao}</CardDescription>
        </CardHeader>
        <CardContent className="pt-2">{children}</CardContent>
      </Card>
    </main>
  )
}
