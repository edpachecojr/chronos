export function EstadoCarregando({ rotulo = "Carregando..." }: { rotulo?: string }) {
  return (
    <div className="flex min-h-32 items-center justify-center">
      <p className="text-sm text-muted-foreground">{rotulo}</p>
    </div>
  )
}
