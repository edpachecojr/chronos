import { defineConfig, devices } from "@playwright/test"

const CONEXAO_BANCO_E2E = "Host=localhost;Port=5432;Database=chronos_agenda_e2e;Username=postgres;Password=postgres"

export default defineConfig({
  testDir: "./e2e",
  fullyParallel: false,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 1 : 0,
  reporter: "list",
  globalSetup: "./e2e/global-setup.ts",
  use: {
    baseURL: "http://localhost:5173",
    trace: "retain-on-failure",
  },
  projects: [
    {
      name: "chromium",
      use: { ...devices["Desktop Chrome"] },
    },
  ],
  webServer: [
    {
      command: "dotnet run --project ../chronos-agenda/src/Chronos.Agenda.Api --no-launch-profile",
      url: "http://localhost:5001/health",
      env: {
        ConnectionStrings__ChronosAgenda: CONEXAO_BANCO_E2E,
        ASPNETCORE_URLS: "http://localhost:5001",
        // Sem isto, appsettings.Development.json (com o Cors:OrigensPermitidas que libera :5173) não carrega,
        // e a Api passa a rejeitar toda requisição cross-origin do frontend (WithOrigins vazio bloqueia tudo).
        ASPNETCORE_ENVIRONMENT: "Development",
      },
      reuseExistingServer: !process.env.CI,
      timeout: 60_000,
      stdout: "pipe",
    },
    {
      command: "pnpm dev",
      url: "http://localhost:5173",
      reuseExistingServer: !process.env.CI,
      timeout: 30_000,
    },
  ],
})
