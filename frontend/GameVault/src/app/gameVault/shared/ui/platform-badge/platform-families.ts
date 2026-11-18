import { IconDefinition } from '@fortawesome/fontawesome-svg-core';
import { faPlaystation, faWindows, faXbox } from '@fortawesome/free-brands-svg-icons';
import { faGamepad, faMobileScreen, faVrCardboard } from '@fortawesome/free-solid-svg-icons';
import { PlatformFamily } from '../../models/game-card.model';

export interface PlatformFamilyStyle {
  label: string;
  icon: IconDefinition;
  toneClass: string;
}

/**
 * Icono, nombre y color de cada familia de plataforma.
 *
 * El backend ya agrupa las plataformas por familia (PlatformFamilyResolver),
 * así que aquí no hace falta adivinar por el nombre ("ps5", "switch"...).
 * Antes estas reglas estaban copiadas en la tarjeta y en el detalle del juego.
 *
 * Nintendo no tiene logotipo libre en Font Awesome: usa el mando.
 */
export const PLATFORM_FAMILIES: Record<PlatformFamily, PlatformFamilyStyle> = {
  PC: { label: 'PC', icon: faWindows, toneClass: 'border-vault-border bg-vault-bg-soft text-vault-text' },
  PlayStation: { label: 'PlayStation', icon: faPlaystation, toneClass: 'border-vault-border bg-vault-bg-soft text-vault-text' },
  Xbox: { label: 'Xbox', icon: faXbox, toneClass: 'border-vault-border bg-vault-bg-soft text-vault-text' },
  Nintendo: { label: 'Nintendo', icon: faGamepad, toneClass: 'border-vault-border bg-vault-bg-soft text-vault-text' },
  Mobile: { label: 'Móvil', icon: faMobileScreen, toneClass: 'border-vault-border bg-vault-bg-soft text-vault-text' },
  VR: { label: 'Realidad virtual', icon: faVrCardboard, toneClass: 'border-vault-border bg-vault-bg-soft text-vault-text' },
  Other: { label: 'Otras', icon: faGamepad, toneClass: 'border-vault-border bg-vault-bg-soft text-vault-text' },
};

