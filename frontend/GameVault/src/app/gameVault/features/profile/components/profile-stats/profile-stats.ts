import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { ProfileStats as Stats } from '../../models/profile.models';

/** Fila de contadores del perfil: juegos, jugando, completados, reseñas y listas. */
@Component({
  selector: 'app-profile-stats',
  changeDetection: ChangeDetectionStrategy.OnPush,
  // Los componentes de Angular son "inline" por defecto; block hace que respeten márgenes y space-y.
  host: { class: 'block' },
  template: `
    <!-- Una sola fila con separadores, sin una caja por número -->
    <dl class="grid grid-cols-3 border-y border-vault-border sm:grid-cols-6">
      @for (stat of items(); track stat.label) {
        <div class="flex flex-col-reverse gap-1 border-vault-border py-4 sm:px-4 sm:not-first:border-l">
          <dt class="text-sm text-vault-muted">{{ stat.label }}</dt>
          <dd class="font-display text-4xl font-extrabold leading-none tabular-nums text-vault-text">{{ stat.value }}</dd>
        </div>
      }
    </dl>
  `,
})
export class ProfileStats {
  readonly stats = input.required<Stats>();

  protected readonly items = computed(() => {
    const stats = this.stats();
    return [
      { label: 'Juegos', value: stats.gamesInLibrary },
      { label: 'Jugando', value: stats.playing },
      { label: 'Completados', value: stats.completed },
      { label: 'Lo quiero', value: stats.wishlist },
      { label: 'Reseñas', value: stats.reviews },
      { label: 'Listas', value: stats.publicLists },
    ];
  });
}
