import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { Icon } from '../../../../shared/ui/icon/icon';
import { IconName } from '../../../../shared/ui/icon/icons';
import { formatReleaseDate } from '../../../../shared/utils/date';
import { normalizeText } from '../../../../shared/utils/text';
import { GameDetailsResponse } from '../../models/games.interfaces';
import { igdbRatingLabel } from '../../../../shared/utils/rating';

interface QuickFact {
  label: string;
  value: string;
  icon: IconName;
}

/** Tarjeta "Ficha del juego": lanzamiento, tipo, nota de IGDB, nota de GameVault y Metacritic. */
@Component({
  selector: 'app-game-quick-facts',
  imports: [Icon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'block' },
  templateUrl: './game-quick-facts.html',
})
export class GameQuickFacts {
  readonly game = input.required<GameDetailsResponse>();

  protected readonly quickFacts = computed<QuickFact[]>(() => {
    const game = this.game();
    const rating = game.rating;
    const metacritic = game.metacritic;

    return [
      { label: 'Lanzamiento', value: formatReleaseDate(game.released), icon: 'calendar' },
      { label: 'Tipo', value: normalizeText(game.gameType) || 'Juego', icon: 'layers' },
      { label: 'IGDB', value: igdbRatingLabel(rating) === 'S/N' ? 'S/N' : `${igdbRatingLabel(rating)}/10`, icon: 'star' },
      {
        label: 'GameVault',
        value: game.communityRating != null ? `${game.communityRating}/10` : 'Sin reseñas',
        icon: 'comment',
      },
      {
        label: 'Metacritic',
        value: typeof metacritic === 'number' && metacritic > 0 ? metacritic.toString() : 'N/A',
        icon: 'gamepad',
      },
    ];
  });
}
