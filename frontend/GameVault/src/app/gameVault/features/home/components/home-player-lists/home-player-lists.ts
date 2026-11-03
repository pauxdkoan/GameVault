import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { GameCard } from '../../../../shared/models/game-card.model';
import { SectionHeader } from '../../../../shared/ui/section-header/section-header';
import { PLAYER_LISTS } from '../../data/home-sample-data';

/** Cuántas portadas se muestran apiladas en cada lista. */
const COVERS_PER_LIST = 3;

/**
 * "Listas creadas por jugadores".
 * Los títulos son de ejemplo; las portadas salen de juegos reales
 * para que la sección tenga vida mientras no exista el backend de listas.
 */
@Component({
  selector: 'app-home-player-lists',
  imports: [SectionHeader],
  templateUrl: './home-player-lists.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomePlayerLists {
  /** Juegos de donde sacar portadas. */
  readonly games = input<GameCard[]>([]);

  protected readonly lists = computed(() => {
    const covers = this.games()
      .map((game) => game.coverImageUrl)
      .filter((cover): cover is string => !!cover);

    return PLAYER_LISTS.map((list, listIndex) => ({
      ...list,
      // Cada lista toma 3 portadas distintas: 0-2, 3-5, 6-8...
      covers: Array.from({ length: COVERS_PER_LIST }, (_, i) =>
        covers.length > 0 ? covers[(listIndex * COVERS_PER_LIST + i) % covers.length] : null,
      ),
    }));
  });
}
