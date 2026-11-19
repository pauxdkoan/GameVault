import {
  ChangeDetectionStrategy,
  Component,
  CUSTOM_ELEMENTS_SCHEMA,
  ElementRef,
  OnDestroy,
  afterRenderEffect,
  computed,
  input,
  signal,
  viewChildren,
} from '@angular/core';
import { GameCardComponent } from '../../../../shared/components/game-card/game-card';
import { GameCardPreview } from '../../../../shared/components/game-card-preview/game-card-preview';
import { calculateGameCardPreviewPosition } from '../../../../shared/components/game-card-preview/game-card-preview-position';
import { GameCard, GameCardDto } from '../../../../shared/models/game-card.model';
import { toGameCard } from '../../../../shared/mappers/game-card.mapper';
import { ensureSwiperRegistered } from '../../../../shared/utils/swiper';

/** Lo mínimo que usamos del elemento <swiper-container>. */
interface SwiperElement extends HTMLElement {
  swiper?: unknown;
  initialize(): void;
}

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
  /**
   * Carrusel infinito (loop) cuando hay juegos suficientes. Con menos, Swiper
   * no tiene diapositivas para dar la vuelta y solo avisaría en consola.
   */
  protected readonly minGamesForLoop = 6;

  /** Los <swiper-container> de la plantilla (#carousel). */
  private readonly carousels = viewChildren<ElementRef<SwiperElement>>('carousel');

  constructor() {
    ensureSwiperRegistered();

    // Angular conecta <swiper-container> al DOM antes de crear sus
    // <swiper-slide>. Si Swiper arranca solo (lo normal), arranca con 0
    // diapositivas y desactiva el loop. Por eso la plantilla usa init="false"
    // y aquí se inicia cada carrusel cuando ya está pintado con sus juegos.
    afterRenderEffect(() => {
      for (const carousel of this.carousels()) {
        if (!carousel.nativeElement.swiper) {
          carousel.nativeElement.initialize();
        }
      }
    });
  }

  readonly similarGames = input<GameCardDto[]>([]);
  readonly dlcs = input<GameCardDto[]>([]);
  readonly expansions = input<GameCardDto[]>([]);
  readonly expandedGames = input<GameCardDto[]>([]);
  readonly activePreview = signal<ActiveGamePreview | null>(null);

  protected readonly sections = computed<DiscoveryCarousel[]>(() =>
    [
      {
        title: 'Juegos similares',
        subtitle: 'Más juegos que conectan con esta experiencia',
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

  /**
   * Usa el mapper compartido y solo cambia el resumen: IGDB no envía
   * resumen de los juegos relacionados, así que se describe con géneros y plataformas.
   */
  private mapRelatedGames(games: GameCardDto[], context: string): GameCard[] {
    return games.map((game) => ({
      ...toGameCard(game),
      summary: this.relatedSummary(game, context),
    }));
  }

  private relatedSummary(game: GameCardDto, context: string): string {
    const genres = game.genres?.filter(Boolean).slice(0, 2).join(', ');
    const platforms = game.platforms?.filter(Boolean).slice(0, 2).join(', ');
    const details = [genres, platforms].filter(Boolean).join(' · ');

    return details
      ? `${context} con datos de ${details}.`
      : `${context} relacionado con este juego.`;
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
