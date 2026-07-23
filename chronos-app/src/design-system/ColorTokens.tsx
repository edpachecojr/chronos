import { cn } from "@/lib/utils"
import type { ColorTone } from "./tokens"

const AA_NORMAL_TEXT_THRESHOLD = 4.5

function ToneSwatch({ tone }: { tone: ColorTone }) {
  const passesAA = tone.contrastOnWhite >= AA_NORMAL_TEXT_THRESHOLD

  return (
    <li className="flex flex-col gap-1.5">
      <div
        className="h-14 w-full rounded-md ring-1 ring-border"
        style={{ backgroundColor: tone.hex }}
        aria-hidden="true"
      />
      <span className="font-mono text-xs text-foreground">
        {tone.variable}
      </span>
      <span className="font-mono text-xs text-muted-foreground">
        {tone.hex}
      </span>
      <span
        className={cn(
          "text-xs",
          passesAA ? "text-neutral-700" : "text-muted-foreground"
        )}
      >
        {tone.contrastOnWhite.toFixed(2)}:1 sobre branco
        {passesAA ? " — AA" : " — reprova AA"}
      </span>
    </li>
  )
}

function ToneScaleSection({
  title,
  tones,
}: {
  title: string
  tones: ColorTone[]
}) {
  return (
    <section className="flex flex-col gap-3">
      <h3 className="text-sm font-medium text-foreground">{title}</h3>
      <ul className="grid grid-cols-2 gap-4 sm:grid-cols-4 lg:grid-cols-6">
        {tones.map((tone) => (
          <ToneSwatch key={tone.variable} tone={tone} />
        ))}
      </ul>
    </section>
  )
}

export function ColorTokens({
  sections,
}: {
  sections: { title: string; tones: ColorTone[] }[]
}) {
  return (
    <div className="flex flex-col gap-8 bg-background p-6">
      {sections.map((section) => (
        <ToneScaleSection key={section.title} {...section} />
      ))}
    </div>
  )
}
