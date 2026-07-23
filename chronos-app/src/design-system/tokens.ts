/**
 * Fonte única dos tokens primitivos de cor documentados em `src/index.css`.
 * Os valores de contraste (WCAG, relativos ao branco #FFFFFF) validam a
 * regra "ação principal precisa de 4.5:1 ou mais": brand-500 reprova
 * (3.68:1) e por isso a ação principal do produto usa brand-600 (5.17:1).
 *
 * @example
 * brandScale.find((tone) => tone.variable === "--brand-600")?.contrastOnWhite
 * // 5.17
 */
export type ColorTone = {
  variable: string
  hex: string
  contrastOnWhite: number
}

export const brandScale: ColorTone[] = [
  { variable: "--brand-50", hex: "#eff6ff", contrastOnWhite: 1.09 },
  { variable: "--brand-100", hex: "#dbeafe", contrastOnWhite: 1.22 },
  { variable: "--brand-200", hex: "#bfdbfe", contrastOnWhite: 1.42 },
  { variable: "--brand-300", hex: "#93c5fd", contrastOnWhite: 1.8 },
  { variable: "--brand-400", hex: "#60a5fa", contrastOnWhite: 2.54 },
  { variable: "--brand-500", hex: "#3b82f6", contrastOnWhite: 3.68 },
  { variable: "--brand-600", hex: "#2563eb", contrastOnWhite: 5.17 },
  { variable: "--brand-700", hex: "#1d4ed8", contrastOnWhite: 6.71 },
  { variable: "--brand-800", hex: "#1e40af", contrastOnWhite: 8.72 },
  { variable: "--brand-900", hex: "#1e3a8a", contrastOnWhite: 10.35 },
  { variable: "--brand-950", hex: "#172554", contrastOnWhite: 14.7 },
]

export const neutralScale: ColorTone[] = [
  { variable: "--neutral-50", hex: "#f8fafc", contrastOnWhite: 1.05 },
  { variable: "--neutral-100", hex: "#f1f5f9", contrastOnWhite: 1.1 },
  { variable: "--neutral-200", hex: "#e2e8f0", contrastOnWhite: 1.23 },
  { variable: "--neutral-300", hex: "#cbd5e1", contrastOnWhite: 1.49 },
  { variable: "--neutral-400", hex: "#94a3b8", contrastOnWhite: 2.56 },
  { variable: "--neutral-500", hex: "#64748b", contrastOnWhite: 4.76 },
  { variable: "--neutral-600", hex: "#475569", contrastOnWhite: 7.58 },
  { variable: "--neutral-700", hex: "#334155", contrastOnWhite: 10.36 },
  { variable: "--neutral-800", hex: "#1e293b", contrastOnWhite: 14.63 },
  { variable: "--neutral-900", hex: "#0f172a", contrastOnWhite: 17.85 },
  { variable: "--neutral-950", hex: "#020617", contrastOnWhite: 20.18 },
]

export const dangerScale: ColorTone[] = [
  { variable: "--danger-600", hex: "#dc2626", contrastOnWhite: 4.83 },
  { variable: "--danger-700", hex: "#b91c1c", contrastOnWhite: 6.47 },
]
