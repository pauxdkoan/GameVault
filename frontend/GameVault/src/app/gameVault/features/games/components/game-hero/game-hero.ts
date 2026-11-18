import { ChangeDetectionStrategy, Component, computed, input, output, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Icon } from '../../../../shared/ui/icon/icon';
import { formatReleaseYear } from '../../../../shared/utils/date';
import { uniqueValues } from '../../../../shared/utils/text';
import { LibraryButton } from '../../../library/components/library-button/library-button';
import { AddToListButton } from '../../../lists/components/add-to-list-button/add-to-list-button';
import { GameDetailsResponse } from '../../models/games.interfaces';

/**
 * Cabecera del detalle de un juego: imagen de fondo, migas de pan, portada,
 * desarrollador y año, nombre, géneros y acciones (biblioteca, lista, nota de la comunidad).
 */
@Component({
  selector: 'app-game-hero',
  imports: [RouterLink, Icon, LibraryButton, AddToListButton],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'block' },
  templateUrl: './game-hero.html',
})
export class GameHero {
  readonly game = input.required<GameDetailsResponse>();
  /** Al pulsar la nota de la comunidad: la página baja a las reseñas. */
  readonly reviewsRequested = output<void>();

  /** Si la portada no carga, se muestra la inicial del juego. */
  protected readonly coverFailed = signal(false);

  /** Primera captura (o la portada) con un degradado encima para que el texto se lea. */
  protected readonly backgroundImage = computed(() => {
    const game = this.game();
    const imageUrl = game.screenshots?.[0] ?? game.coverImage ?? null;

    if (!imageUrl) {
      return 'linear-gradient(180deg, #101215, #0b0c0e)';
    }

    return `linear-gradient(90deg, rgba(11, 12, 14, 0.95) 0%, rgba(11, 12, 14, 0.72) 45%, rgba(11, 12, 14, 0.35) 100%), linear-gradient(180deg, rgba(11, 12, 14, 0.2) 0%, rgba(11, 12, 14, 0.92) 100%), url("${imageUrl}")`;
  });

  protected readonly releaseYear = computed(() => formatReleaseYear(this.game().released));

  protected readonly primaryDeveloper = computed(() => {
    const developers = uniqueValues(
      this.game()
        .companies.filter((company) => company.isDeveloper)
        .map((company) => company.name),
    );

    return developers.join(', ') || 'Desarrollador por confirmar';
  });

  protected readonly genreChips = computed(() => uniqueValues(this.game().genres ?? []).slice(0, 5));
}
