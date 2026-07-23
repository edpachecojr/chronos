import { CalendarClock, Clock3, Scissors, Settings } from "lucide-react"
import { NavLink } from "react-router-dom"

import { cn } from "@/lib/utils"

interface ItemNavegacao {
  rota: string
  rotulo: string
  Icone: typeof CalendarClock
  fim?: boolean
}

const itensPrincipais: ItemNavegacao[] = [
  { rota: "/", rotulo: "Agenda", Icone: CalendarClock, fim: true },
  { rota: "/servicos", rotulo: "Serviços", Icone: Scissors },
  { rota: "/disponibilidade", rotulo: "Disponibilidade", Icone: Clock3 },
]

const itemConfiguracoes: ItemNavegacao = {
  rota: "/configuracoes",
  rotulo: "Configurações",
  Icone: Settings,
}

function LinkNavegacao({ rota, rotulo, Icone, fim, aoNavegar }: ItemNavegacao & { aoNavegar?: () => void }) {
  return (
    <NavLink
      to={rota}
      end={fim}
      onClick={aoNavegar}
      className={({ isActive }) =>
        cn(
          "flex items-center gap-x-3 rounded-md px-3 py-2 text-sm font-medium transition-colors",
          isActive
            ? "bg-sidebar-primary text-sidebar-primary-foreground"
            : "text-sidebar-foreground/70 hover:bg-sidebar-accent hover:text-sidebar-accent-foreground"
        )
      }
    >
      <Icone className="size-5 shrink-0" aria-hidden="true" />
      {rotulo}
    </NavLink>
  )
}

interface AppSidebarProps {
  aoNavegar?: () => void
}

/** Menu lateral persistente: logo e navegação entre as páginas da área logada. */
export function AppSidebar({ aoNavegar }: AppSidebarProps) {
  return (
    <div className="flex h-full w-full flex-col bg-sidebar text-sidebar-foreground">
      <div className="flex h-16 shrink-0 items-center gap-2.5 border-b border-sidebar-border px-5">
        <img src="/favicon.svg" alt="" className="size-7" aria-hidden="true" />
        <span className="font-heading text-base font-semibold tracking-tight">Chronos</span>
      </div>

      <nav className="flex flex-1 flex-col gap-y-1 overflow-y-auto px-3 py-4">
        {itensPrincipais.map((item) => (
          <LinkNavegacao key={item.rota} {...item} aoNavegar={aoNavegar} />
        ))}
        <div className="mt-auto">
          <LinkNavegacao {...itemConfiguracoes} aoNavegar={aoNavegar} />
        </div>
      </nav>
    </div>
  )
}
