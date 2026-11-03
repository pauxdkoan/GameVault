import {
  ChangeDetectionStrategy,
  Component,
  CUSTOM_ELEMENTS_SCHEMA,
  OnDestroy,
  computed,
  input,
  signal,
} from '@angular/core';
import { GameCardComponent } from '../../../../shared/components/game-card/game-card';
import { GameCardPreview } from '../../../../shared/components/game-card-preview/game-card-preview';
import { calculateGameCardPreviewPosition } from '../../../../shared/components/game-card-preview/game-card-preview-position';
import { GameCard } from '../../../../shared/models/game-card.model';
import { Dlc } from '../../models/games.interfaces';

interface DiscoveryCarousel {
  title: string;
  subtitle: string;
  games: GameCard[];
}

interface ActiveGamePreview {
  game: GameCard;
  top: number;
  left: number;
}

@Component({
  selector: 'app-game-discover-more',
  imports: [GameCardComponent, GameCardPreview],
  templateUrl: './game-discover-more.html',
  styles: [],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class GameDiscoverMoreComponent implements OnDestroy {
  readonly similarGames = input<Dlc[]>([]);
  readonly dlcs = input<Dlc[]>([]);
  readonly expansions = input<Dlc[]>([]);
  readonly expandedGames = input<Dlc[]>([]);
  readonly activePreview = signal<ActiveGamePreview | null>(null);

  protected readonly sections = computed<DiscoveryCarousel[]>(() =>
    [
      {
        title: 'Juegos similares',
        subtitle: 'Mas juegos que conectan con esta experiencia',
        games: this.mapRelatedGames(this.similarGames(), 'Juego similar'),
      },
      {
        title: 'DLC y expansiones',
        subtitle: 'Contenido adicional relacionado',
        games: this.mapRelatedGames(
          [...this.dlcs(), ...this.expansions()],
          'Contenido adicional',
        ),
      },
      {
        title: 'Mas de esta familia',
        subtitle: 'Versiones o juegos expandidos',
        games: this.mapRelatedGames(this.expandedGames(), 'Juego relacionado'),
      },
    ].filter((section) => section.games.length > 0),
  );

  private mapRelatedGames(games: Dlc[], fallbackContext: string): GameCard[] {
    return games
      .filter((game) => game.externalId && game.name?.trim())
      .map((game) => ({
        id: game.externalId.toString(),
        externalId: game.externalId,
        name: game.name,
        slug: this.slugify(game.name),
        summary: this.relatedSummary(game, fallbackContext),
        releaseLabel: fallbackContext,
        coverImageUrl: game.coverImage,
        screenshotUrl: game.coverImage,
        screenshotUrls: game.coverImage ? [game.coverImage] : [],
        trailerVideoUrl: null,
        rating: game.raiting ?? 0,
        metacritic: null,
        genres: game.genres?.length ? game.genres.slice(0, 3) : ['Sin genero'],
        platformLabel: game.platforms ?? [],
      }));
  }

  private relatedSummary(game: Dlc, fallbackContext: string): string {
    const genres = game.genres?.filter(Boolean).slice(0, 2).join(', ');
    const platforms = game.platforms?.filter(Boolean).slice(0, 2).join(', ');
    const details = [genres, platforms].filter(Boolean).join(' · ');

    return details
      ? `${fallbackContext} con datos de ${details}.`
      : `${fallbackContext} relacionado con este juego.`;
  }

  private slugify(value: string): string {
    return value
      .toLowerCase()
      .trim()
      .replace(/[^a-z0-9]+/g, '-')
      .replace(/^-+|-+$/g, '');
  }

  private closePreviewTimeout: ReturnType<typeof setTimeout> | null = null;

  ngOnDestroy(): void {
    this.cancelClosePreview();
  }

  protected scheduleClosePreview(): void {
    this.cancelClosePreview();

    this.closePreviewTimeout = setTimeout(() => {
      this.activePreview.set(null);
      this.closePreviewTimeout = null;
    }, 150);
  }

  protected showGamePreview(game: GameCard, element: HTMLElement): void {
    this.cancelClosePreview();

    const position = calculateGameCardPreviewPosition(element);

    this.activePreview.set({
      game,
      ...position,
    });
  }

  protected cancelClosePreview(): void {
    if (!this.closePreviewTimeout) {
      return;
    }

    clearTimeout(this.closePreviewTimeout);

    this.closePreviewTimeout = null;
  }
}
