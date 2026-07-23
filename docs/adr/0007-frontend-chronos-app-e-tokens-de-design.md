# ADR 0007: Estrutura do frontend chronos-app e sistema de tokens de design

- Status: Aceito
- Data: 2026-07-22

## Contexto

O ADR 0001 definiu apenas a stack de frontend (React 19, Tailwind CSS,
shadcn/ui, Lucide React), sem código, estrutura de diretórios ou sistema de
cores. Era necessário criar o projeto `chronos-app` e decidir: a ferramenta de
build, a tipografia, a paleta de cores e sua validação de acessibilidade, e
como documentar os componentes.

O produto deve ter visual minimalista, elegante e moderno, com superfície
sempre branca (sem tema escuro) e tipografia Inter.

## Decisão

### Build e stack

- `chronos-app` usa Vite (não Next.js): é uma SPA que consome a Api do
  `chronos-agenda` via REST, sem necessidade de SSR/roteamento de arquivos.
- React 19 + TypeScript, Tailwind CSS v4 (configuração CSS-first via
  `@theme`, sem `tailwind.config.js`), shadcn/ui (estilo `radix-nova`,
  ícones Lucide) e Storybook 10 para documentação viva dos componentes.
- Gerenciador de pacotes: pnpm, fixado via `packageManager` em
  `package.json`. O projeto começou com npm; a troca para pnpm não altera
  nenhuma decisão de stack, apenas o lockfile (`pnpm-lock.yaml`) e a forma
  de invocar os scripts (`pnpm <script>`, `pnpm dlx shadcn add`).
- Tipografia: Inter, via `@fontsource-variable/inter` (self-hosted), evitando
  chamadas a uma CDN de fontes externa.
- Alias de import `@/*` → `src/*`, compartilhado entre `tsconfig`, Vite e
  shadcn/ui.

### Tokens de cor

- A Key Color `#2563EB` coincide com o tom 600 de uma escala de 11 tons
  (50–950); a escala inteira foi construída em torno dela e documentada em
  `chronos-app/src/index.css` e `chronos-app/src/design-system/tokens.ts`.
- Validação WCAG AA (contraste sobre branco): brand-500 reprova (3.68:1);
  por isso a ação principal usa brand-600 (5.17:1), não brand-500.
- Uma escala neutra (cool gray, independente da Key Color) cobre
  superfícies, bordas e tipografia do corpo do texto — um sistema só com
  tons de marca tingiria toda a interface de azul.
- Os tokens semânticos são mapeados diretamente no contrato de variáveis do
  shadcn/ui (`--background`, `--primary`, `--ring` etc.) em vez de uma
  camada paralela com nomes próprios, para não duplicar a mesma decisão em
  dois lugares (DRY).
- `--ring` (foco) usa o mesmo tom de `--primary` (brand-600), não o tom
  900–950: mantém o mesmo matiz da ação primária e ainda ultrapassa com
  folga o mínimo de contraste não textual (3:1) exigido pelo WCAG 2.4.11
  para indicadores de foco.
- `--destructive` usa danger-700 (não danger-600): o Button/Badge do
  shadcn/ui aplica esse token como texto sobre um fundo "soft"
  (`bg-destructive/10`), não sobre branco puro; danger-600 reprovava AA
  nesse fundo (4.13:1), confirmado por um teste de acessibilidade real no
  Storybook. Contra branco puro os dois tons já passavam.
- Não há tema escuro: o bloco `.dark` do preset padrão do shadcn/ui foi
  removido. A variante `dark:` de Tailwind permanece como variante de
  classe (`@custom-variant dark (&:is(.dark *))`), não de
  `prefers-color-scheme`, porque alguns componentes gerados pelo shadcn/ui
  (Button, Badge) usam classes `dark:` internamente; como a aplicação nunca
  aplica a classe `.dark`, essas classes ficam inertes em vez de reagir à
  preferência do sistema operacional do usuário.

### Testes e documentação

- Cada componente novo ganha uma story (`*.stories.tsx`). O addon
  `@storybook/addon-vitest` roda cada story como teste no Chromium via
  Playwright, e o addon `@storybook/addon-a11y` (configurado como
  `test: 'error'`) falha a suíte se qualquer story violar WCAG — foi assim
  que a inconsistência do `--destructive` acima foi encontrada.
- Comando único de teste: `pnpm test` (`vitest --project=storybook run`),
  registrado também em `CLAUDE.md`.
- `src/design-system/tokens.ts` e `ColorTokens.stories.tsx` documentam a
  paleta completa como uma página viva no Storybook (`Design System/Tokens/
  Cores`), com o hex e o contraste calculado de cada tom.

## Consequências

- `pnpm-workspace.yaml` tem um campo `overrides` fixando `aria-query` em
  `5.3.2`: sem isso, `@testing-library/dom` resolve `aria-query@5.3.0`
  (versão com bug de interoperabilidade CJS/ESM) e o Vitest em modo
  navegador falha ao importar o setup do addon de testes do Storybook.
  Isso é específico da combinação de versões atual do Storybook/Vitest e
  pode ser revisitado quando essas bibliotecas corrigirem a
  interoperabilidade CJS/ESM no upstream.
- Novos componentes shadcn/ui adicionados via `pnpm dlx shadcn add` herdam
  os tokens automaticamente, sem precisar de ajuste manual de cor.
- Qualquer novo uso de `--destructive` (ou de `danger-600`/`danger-700`
  diretamente) sobre um fundo que não seja branco puro deve reconferir o
  contraste; a suíte de testes falha automaticamente se isso quebrar AA em
  uma story existente.

## Alternativas consideradas

### Next.js em vez de Vite

Rejeitado por ora: nenhum requisito atual de SSR, SEO ou rotas de arquivo
justifica a complexidade adicional. Pode ser revisto se o produto precisar
de renderização no servidor.

### Camada de tokens semânticos própria, paralela ao contrato do shadcn/ui

Rejeitada: duplicaria a mesma decisão (ex.: `--color-action-primary` e
`--primary` apontando para o mesmo valor), violando DRY, sem nenhum
consumidor que precise dos dois nomes.

### Tema escuro automático por `prefers-color-scheme`

Rejeitado por requisito explícito do produto (superfície sempre branca). A
variante `dark:` do Tailwind foi mantida apenas como classe inerte para não
exigir alterações nos componentes gerados pelo shadcn/ui.
