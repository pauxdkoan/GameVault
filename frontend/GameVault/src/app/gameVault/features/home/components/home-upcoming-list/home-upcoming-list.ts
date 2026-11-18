import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { GameCard } from '../../../../shared/models/game-card.model';
import { SectionHeader } from '../../../../shared/ui/section-header/section-header';
import { Icon } from '../../../../shared/ui/icon/icon';

/**
 * Próximos lanzamientos como un calendario: la fecha delante, en grande,
 * y después la portada y el nombre. Una fila por juego, separadas por líneas.
 */
@Component({
  selector: 'app-home-upcoming-list',
  imports: [Icon, RouterLink, SectionHeader],
  templateUrl: './home-upcoming-list.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomeUpcomingList {
  readonly games = input.required<GameCard[]>();
  readonly maxGames = input(5);

  protected readonly rows = computed(() => this.games().slice(0, this.maxGames()));
}
