import { execFileSync } from "node:child_process"

const CONEXAO_BANCO_E2E = "Host=localhost;Port=5432;Database=chronos_agenda_e2e;Username=postgres;Password=postgres"

/**
 * Aplica as migrations do chronos-agenda contra um banco isolado (`chronos_agenda_e2e`), separado do banco de
 * desenvolvimento, antes de toda a suíte E2E rodar. O EF Core cria o banco automaticamente na primeira migration
 * se ele ainda não existir.
 */
export default function globalSetup() {
  execFileSync(
    "dotnet",
    [
      "ef",
      "database",
      "update",
      "--project",
      "../chronos-agenda/src/Chronos.Agenda.Infrastructure",
      "--startup-project",
      "../chronos-agenda/src/Chronos.Agenda.Api",
    ],
    {
      env: { ...process.env, ConnectionStrings__ChronosAgenda: CONEXAO_BANCO_E2E },
      stdio: "inherit",
    },
  )
}
