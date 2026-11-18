import { ChangeDetectionStrategy, Component, input } from '@angular/core';

/**
 * Cabecera de las páginas de usuario (biblioteca, listas, ajustes...).
 * Lo que se ponga dentro (<app-page-header>botones</app-page-header>)
 * aparece a la derecha, para acciones como "Nueva lista".
 */
@Component({
  selector: 'app-page-header',
  changeDetection: ChangeDetectionStrategy.OnPush,
  // Los componentes de Angular son "inline" por defecto; block hace que respeten márgenes y space-y.
  host: { class: 'block' },
  template: `
    <header class="flex flex-wrap items-end justify-between gap-5">
      <div class="max-w-2xl">
        <h1 class="text-5xl text-vault-text sm:text-6xl">{{ title() }}</h1>
        @if (subtitle()) {
          <p class="mt-3 text-base leading-7 text-vault-muted">{{ subtitle() }}</p>
        }
      </div>
      <div class="flex flex-wrap items-center gap-3">
        <ng-content />
      </div>
    </header>
  `,
})
export class PageHeader {
  readonly title = input.required<string>();
  readonly subtitle = input('');
}
