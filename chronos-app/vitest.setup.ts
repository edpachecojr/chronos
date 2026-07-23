import "@testing-library/jest-dom/vitest"

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
