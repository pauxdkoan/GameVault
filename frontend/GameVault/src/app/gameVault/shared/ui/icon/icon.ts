import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { ICONS, IconName } from './icons';

/**
 * Icono de interfaz. El tamaño y el color se ponen con clases de Tailwind:
 *
 *   <app-icon name="heart" class="size-4 text-vault-danger" />
 *   <app-icon name="heart" class="size-4" filled />      (relleno)
 *   <app-icon name="check" class="size-3" [strokeWidth]="3" />
 *
 * Siempre lleva una clase size-*: el SVG ocupa todo el tamaño del componente.
 * Es decorativo (aria-hidden): el texto accesible va en el botón o enlace que lo contiene.
 */
@Component({
  selector: 'app-icon',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'inline-block shrink-0', 'aria-hidden': 'true' },
  template: `
    <svg
      class="block h-full w-full"
      viewBox="0 0 24 24"
      [attr.fill]="filled() ? 'currentColor' : 'none'"
      stroke="currentColor"
      [attr.stroke-width]="strokeWidth()"
      stroke-linecap="round"
      stroke-linejoin="round"
    >
      @for (d of paths(); track $index) {
        <path [attr.d]="d" />
      }
    </svg>
  `,
})
export class Icon {
  readonly name = input.required<IconName>();
  readonly strokeWidth = input(2);
  /** Rellena la figura (por ejemplo, el corazón de "me gusta" activo). */
  readonly filled = input(false, { transform: (value: boolean | string) => value !== false && value !== 'false' });

  protected readonly paths = computed(() => ICONS[this.name()]);
}
