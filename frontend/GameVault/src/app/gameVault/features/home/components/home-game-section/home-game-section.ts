import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { GameCardComponent } from '../../../../shared/components/game-card/game-card';
import { SectionHeader } from '../../../../shared/ui/section-header/section-header';
import { GameCard } from '../../../../shared/models/game-card.model';

/**
 * Sección con una fila de tarjetas de juego (ej: "Tendencias esta semana").
 * Muestra como máximo `maxGames` juegos.
 */
@Component({
  selector: 'app-home-game-section',
  imports: [GameCardComponent, SectionHeader],
  templateUrl: './home-game-section.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomeGameSectionComponent {
  readonly sectionId = input.required<string>();
  readonly title = input.required<string>();
  readonly subtitle = input<string | null>(null);
  readonly games = input.required<GameCard[]>();
  readonly maxGames = input(6);

  protected readonly visibleGames = computed(() =>
    this.games().slice(0, this.maxGames()),
  );
}
