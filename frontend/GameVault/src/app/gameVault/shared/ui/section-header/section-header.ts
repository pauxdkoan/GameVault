import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

/**
 * Encabezado de sección: título, subtítulo y un enlace opcional
 * a la derecha ("Ver todos los juegos →").
 */
@Component({
  selector: 'app-section-header',
  imports: [RouterLink],
  template: `
    <div class="flex flex-wrap items-end justify-between gap-4">
      <div class="max-w-2xl">
        <h2 class="text-3xl text-vault-text sm:text-4xl">
          {{ title() }}
        </h2>
        @if (subtitle()) {
          <p class="mt-2 text-sm text-vault-muted sm:text-base">
            {{ subtitle() }}
          </p>
        }
      </div>

      @if (linkLabel() && linkRoute()) {
        <a
          class="group inline-flex items-center gap-1.5 text-sm font-semibold text-vault-accent transition hover:text-vault-accent-soft"
          [routerLink]="linkRoute()"
        >
          {{ linkLabel() }}
          <span
            class="transition group-hover:translate-x-0.5"
            aria-hidden="true"
            >→</span
          >
        </a>
      }
    </div>
  `,
})
export class SectionHeader {
  readonly title = input.required<string>();
  readonly subtitle = input<string | null>(null);
  readonly linkLabel = input<string | null>(null);
  readonly linkRoute = input<string | null>(null);
}
