import * as React from "react"
import { Eye, EyeOff } from "lucide-react"

import { cn } from "@/lib/utils"
import { Input } from "@/components/ui/input"

function PasswordInput({ className, ...props }: React.ComponentProps<"input">) {
  const [visivel, setVisivel] = React.useState(false)

  return (
    <div className="relative">
      <Input type={visivel ? "text" : "password"} className={cn("pr-8", className)} {...props} />
      <button
        type="button"
        onClick={() => setVisivel((atual) => !atual)}
        className="absolute inset-y-0 right-0 flex w-8 items-center justify-center text-muted-foreground outline-none hover:text-foreground focus-visible:text-foreground"
        aria-label={visivel ? "Ocultar senha" : "Mostrar senha"}
        tabIndex={-1}
      >
        {visivel ? (
          <EyeOff className="size-4" aria-hidden="true" />
        ) : (
          <Eye className="size-4" aria-hidden="true" />
        )}
      </button>
    </div>
  )
}

export { PasswordInput }
