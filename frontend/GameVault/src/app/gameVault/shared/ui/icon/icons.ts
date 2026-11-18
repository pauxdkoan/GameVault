/**
 * Iconos de interfaz de GameVault, en un solo sitio.
 *
 * Todos se dibujan en una cuadrícula de 24x24 con trazo (stroke), así
 * comparten grosor y estilo. Para añadir uno nuevo basta con una entrada más.
 *
 * Los logotipos de plataformas (PlayStation, Xbox...) NO van aquí: son
 * marcas y salen de Font Awesome en shared/ui/platform-badge.
 */
export const ICONS = {
  search: ['M11 18a7 7 0 1 0 0-14 7 7 0 0 0 0 14Z', 'm21 21-4.35-4.35'],
  check: ['M5 12l5 5 9-10'],
  plus: ['M12 5v14M5 12h14'],
  x: ['M18 6 6 18M6 6l12 12'],
  'chevron-down': ['m6 9 6 6 6-6'],
  'chevron-left': ['m15 18-6-6 6-6'],
  'chevron-right': ['m9 18 6-6-6-6'],
  'arrow-right': ['M5 12h14M13 6l6 6-6 6'],
  external: ['M15 3h6v6M10 14 21 3', 'M18 13v6a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h6'],
  play: ['M12 22a10 10 0 1 0 0-20 10 10 0 0 0 0 20Z', 'M10 8.5v7l6-3.5-6-3.5Z'],
  calendar: ['M5 5h14a2 2 0 0 1 2 2v12a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V7a2 2 0 0 1 2-2Z', 'M16 3v4M8 3v4M3 10h18'],
  star: ['M12 3l2.8 5.7 6.2.9-4.5 4.4 1 6.2L12 17.3l-5.5 2.9 1-6.2L3 9.6l6.2-.9L12 3Z'],
  heart: ['M20.8 4.6a5.5 5.5 0 0 0-7.8 0L12 5.7l-1-1.1a5.5 5.5 0 1 0-7.8 7.8L12 21l8.8-8.6a5.5 5.5 0 0 0 0-7.8Z'],
  comment: ['M21 12a8 8 0 0 1-11.6 7.1L3 21l1.9-6.4A8 8 0 1 1 21 12Z'],
  layers: ['m12 3 9 5-9 5-9-5 9-5Z', 'm3 13 9 5 9-5'],
  gamepad: [
    'M6 11h4M8 9v4M15 12h.01M18 10h.01',
    'M17.3 5H6.7a4 4 0 0 0-4 3.6l-.7 6.3A3 3 0 0 0 5 18c1 0 1.5-.5 2-1l1.4-1.4a2 2 0 0 1 1.4-.6h4.4a2 2 0 0 1 1.4.6L17 17c.5.5 1 1 2 1a3 3 0 0 0 3-3.1l-.7-6.3A4 4 0 0 0 17.3 5Z',
  ],
  'list-plus': ['M4 6h16M4 12h10M4 18h7M17 15v6M14 18h6'],
  eye: ['M2 12c1-3.5 5-8 10-8s9 4.5 10 8c-1 3.5-5 8-10 8S3 15.5 2 12Z', 'M12 15a3 3 0 1 0 0-6 3 3 0 0 0 0 6Z'],
  'eye-off': [
    'M3 3l18 18',
    'M10.6 10.6a2 2 0 0 0 2.8 2.8',
    'M9.9 4.2A10.4 10.4 0 0 1 12 4c5 0 9 4.5 10 8-.4 1.3-1.2 2.7-2.3 4M6.6 6.6C4.5 8 2.9 10 2 12c1 3.5 5 8 10 8 1.7 0 3.3-.5 4.7-1.3',
  ],
} satisfies Record<string, string[]>;

export type IconName = keyof typeof ICONS;
