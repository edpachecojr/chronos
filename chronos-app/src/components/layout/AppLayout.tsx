import { ChevronDown, LogOut, Menu, X } from "lucide-react"
import { useEffect, useState } from "react"
import { Outlet } from "react-router-dom"

import { AppSidebar } from "@/components/layout/AppSidebar"
import { Button } from "@/components/ui/button"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { Separator } from "@/components/ui/separator"
import { useAuth } from "@/hooks/useAuth"
import { cn } from "@/lib/utils"

/** Extrai até duas iniciais de um nome para exibir como avatar textual da organização. */
function iniciaisOrganizacao(nome: string | undefined) {
  if (!nome) return ""
  return nome
    .trim()
    .split(/\s+/)
    .slice(0, 2)
    .map((parte) => parte[0]?.toUpperCase() ?? "")
    .join("")
}

/** Layout persistente da área logada: menu lateral + barra superior com perfil + conteúdo restrito. */
export function AppLayout() {
  const { organizacao, sair } = useAuth()
  const [menuAberto, setMenuAberto] = useState(false)

  useEffect(() => {
    if (!menuAberto) return

    function aoPressionarTecla(evento: KeyboardEvent) {
      if (evento.key === "Escape") setMenuAberto(false)
    }

    window.addEventListener("keydown", aoPressionarTecla)
    return () => window.removeEventListener("keydown", aoPressionarTecla)
  }, [menuAberto])

  return (
    <div className="flex h-svh overflow-hidden">
      <aside className="hidden border-r border-sidebar-border md:flex md:w-72 md:shrink-0">
        <AppSidebar />
      </aside>

      <div
        className={cn(
          "fixed inset-0 z-40 bg-neutral-950/50 transition-opacity md:hidden",
          menuAberto ? "opacity-100" : "pointer-events-none opacity-0"
        )}
        onClick={() => setMenuAberto(false)}
        aria-hidden="true"
      />

      <aside
        id="menu-navegacao"
        className={cn(
          "fixed inset-y-0 left-0 z-50 w-72 shadow-lg shadow-neutral-900/10 transition-transform md:hidden",
          menuAberto ? "translate-x-0" : "-translate-x-full"
        )}
      >
        <AppSidebar aoNavegar={() => setMenuAberto(false)} />
      </aside>

      <div className="flex min-w-0 flex-1 flex-col">
        <header className="sticky top-0 z-30 flex h-16 shrink-0 items-center gap-x-4 border-b border-border bg-background px-4 sm:px-6">
          <Button
            variant="ghost"
            size="icon"
            className="md:hidden"
            aria-label={menuAberto ? "Fechar menu" : "Abrir menu"}
            aria-expanded={menuAberto}
            aria-controls="menu-navegacao"
            onClick={() => setMenuAberto((aberto) => !aberto)}
          >
            {menuAberto ? <X aria-hidden="true" /> : <Menu aria-hidden="true" />}
          </Button>

          <Separator orientation="vertical" className="h-6 md:hidden" />

          <div className="flex flex-1 items-center justify-end">
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <button
                  type="button"
                  className="flex items-center gap-x-2 rounded-md py-1.5 outline-none focus-visible:ring-3 focus-visible:ring-ring/50"
                >
                  <span
                    className="flex size-8 shrink-0 items-center justify-center rounded-full bg-accent text-sm font-semibold text-accent-foreground"
                    aria-hidden="true"
                  >
                    {iniciaisOrganizacao(organizacao?.nome)}
                  </span>
                  <span className="hidden items-center gap-x-1 sm:flex">
                    <span className="text-sm font-medium text-foreground">{organizacao?.nome}</span>
                    <ChevronDown className="size-4 text-muted-foreground" aria-hidden="true" />
                  </span>
                  <span className="sr-only">Abrir menu do perfil</span>
                </button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end">
                <DropdownMenuItem onSelect={sair}>
                  <LogOut aria-hidden="true" />
                  Sair
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
          </div>
        </header>

        <main className="flex-1 overflow-y-auto bg-muted p-4 sm:p-6 lg:p-8">
          <Outlet />
        </main>
      </div>
    </div>
  )
}
