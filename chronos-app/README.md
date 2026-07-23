# chronos-app

Frontend do Chronos: React 19, TypeScript, Vite, Tailwind CSS e shadcn/ui.

Gerenciador de pacotes: pnpm (`packageManager` fixado em `package.json`).

## Comandos

- `pnpm dev` — servidor de desenvolvimento.
- `pnpm build` — build de produção (`dist/`).
- `pnpm lint` — lint (oxlint).
- `pnpm test` — testes de componentes e acessibilidade (Storybook + Vitest).
- `pnpm storybook` — documentação viva dos componentes.

## Sistema de tokens de design

Os tokens primitivos (`--brand-*`, `--neutral-*`, `--danger-*`) e semânticos
(`--primary`, `--background`, `--ring` etc.) ficam em `src/index.css` e estão
documentados, com os valores de contraste WCAG calculados, em
`src/design-system/tokens.ts` e na story `Design System/Tokens/Cores` do
Storybook. As decisões e a validação de acessibilidade estão registradas em
[`docs/adr/0007-frontend-chronos-app-e-tokens-de-design.md`](../docs/adr/0007-frontend-chronos-app-e-tokens-de-design.md).

Componentes de UI (`src/components/ui`) vêm do shadcn/ui e devem ser
adicionados ou atualizados via `pnpm dlx shadcn add <componente>`, não
editados manualmente na estrutura de pastas.
