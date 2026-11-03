import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { GameCard } from '../../../../shared/models/game-card.model';
import { SectionHeader } from '../../../../shared/ui/section-header/section-header';

/** Etiquetas de colores que se van alternando en cada fila. */
const ROW_LABELS = [
  { text: 'Este mes', classes: 'text-vault-cyan' },
  { text: 'Más esperado', classes: 'text-vault-violet' },
  { text: 'Próximamente', classes: 'text-vault-success' },
];

/** Lista de próximos lanzamientos en filas horizontales. */
@Component({
  selector: 'app-home-upcoming-list',
  imports: [RouterLink, SectionHeader],
  templateUrl: './home-upcoming-list.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomeUpcomingList {
  readonly games = input.required<GameCard[]>();
  readonly maxGames = input(3);

  protected readonly rows = computed(() =>
    this.games()
      .slice(0, this.maxGames())
      .map((game, index) => ({
        game,
        label: ROW_LABELS[index % ROW_LABELS.length],
      })),
  );
}
