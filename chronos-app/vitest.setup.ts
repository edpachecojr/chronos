import "@testing-library/jest-dom/vitest"

import { cleanup } from "@testing-library/react"
import { afterEach } from "vitest"

/** Sem `test.globals: true`, @testing-library/react não detecta o `afterEach` do vitest automaticamente para
 * desmontar o DOM entre testes — sem isto, elementos de um teste vazam para o próximo e quebram buscas por
 * texto/role duplicados. */
afterEach(() => {
  cleanup()
})

/**
 * jsdom não implementa a Pointer Events API nem `scrollIntoView`; componentes radix-ui (Select, Dropdown, ...)
 * as usam internamente e lançam em ambiente de teste sem este polyfill.
 */
if (!Element.prototype.hasPointerCapture) {
  Element.prototype.hasPointerCapture = () => false
}
if (!Element.prototype.setPointerCapture) {
  Element.prototype.setPointerCapture = () => {}
}
if (!Element.prototype.releasePointerCapture) {
  Element.prototype.releasePointerCapture = () => {}
}
if (!Element.prototype.scrollIntoView) {
  Element.prototype.scrollIntoView = () => {}
}
