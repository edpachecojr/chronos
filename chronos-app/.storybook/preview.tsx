import type { Preview } from '@storybook/react-vite'

import '../src/index.css'

const preview: Preview = {
  parameters: {
    controls: {
      matchers: {
       color: /(background|color)$/i,
       date: /Date$/i,
      },
    },

    a11y: {
      // O contraste WCAG AA é um requisito do sistema de tokens (ver
      // src/index.css), não apenas uma verificação opcional: falhas de
      // acessibilidade devem quebrar a build do Storybook.
      test: 'error'
    }
  },
};

export default preview;