import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { UserAvatar } from '../../../../shared/ui/user-avatar/user-avatar';
import { Icon } from '../../../../shared/ui/icon/icon';
import { GameListSummary } from '../../models/list.models';

/**
 * Tarjeta de una lista: hasta 4 portadas apiladas, nombre, autor y número de juegos.
 * Se usa en "Mis listas", en el perfil y en el home.
 */
@Component({
  selector: 'app-list-card',
  imports: [RouterLink, UserAvatar, Icon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  // Los componentes de Angular son "inline" por defecto; block hace que respeten márgenes y space-y.
  host: { class: 'block' },
  template: `
    @let item = list();
    <a
      class="vault-card group block overflow-hidden transition hover:border-vault-accent/60 active:scale-[0.99]"
      [routerLink]="['/gamevault/lists', item.id]"
    >
      <!-- Portadas apiladas como cajas en una estantería: se adapta a 1, 2, 3 o 4 juegos -->
      <div class="flex h-40 items-end border-b border-vault-border bg-vault-bg-soft px-4 pt-4">
        @for (cover of covers(); track $index; let first = $first) {
          <img
            class="aspect-[2/3] h-32 rounded-t-md border border-b-0 border-vault-border-strong object-cover transition group-hover:-translate-y-1"
            [class.-ml-8]="!first"
            [src]="cover"
            alt=""
            loading="lazy"
          />
        } @empty {
          <span class="mb-6 flex w-full flex-col items-center gap-2 text-sm text-vault-subtle">
            <app-icon name="list-plus" class="size-6" />
            Lista vacía
          </span>
        }
      </div>
      <div class="p-4">
        <div class="flex items-start justify-between gap-2">
          <h3 class="line-clamp-1 text-lg font-semibold text-vault-text group-hover:text-vault-accent">{{ item.name }}</h3>
          @if (!item.isPublic) {
            <span class="vault-tag shrink-0 py-0.5">Privada</span>
          }
        </div>
        @if (item.description) {
          <p class="mt-1 line-clamp-2 text-sm text-vault-muted">{{ item.description }}</p>
        }
        <div class="mt-3 flex items-center justify-between text-sm text-vault-subtle">
          @if (showOwner()) {
            <span class="flex items-center gap-2">
              <app-user-avatar [name]="item.owner.displayName" [avatarUrl]="item.owner.avatarUrl" [size]="22" />
              {{ item.owner.displayName }}
            </span>
          }
          <span class="tabular-nums">{{ item.gameCount }} {{ item.gameCount === 1 ? 'juego' : 'juegos' }}</span>
        </div>
      </div>
    </a>
  `,
})
export class ListCard {
  readonly list = input.required<GameListSummary>();
  /** En "Mis listas" el autor soy yo, así que no hace falta mostrarlo. */
  readonly showOwner = input(true);

  /** Hasta 4 portadas; si no hay ninguna se muestra "Lista vacía". */
  protected readonly covers = computed(() => this.list().coverImages.filter(Boolean).slice(0, 4));
}
