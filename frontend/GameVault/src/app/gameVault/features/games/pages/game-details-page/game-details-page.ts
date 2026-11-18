import { ChangeDetectionStrategy, Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { take } from 'rxjs';
import { errorMessages } from '../../../../core/http/api-error';
import { normalizeText, uniqueValues } from '../../../../shared/utils/text';
import { GameReviews } from '../../../reviews/components/game-reviews/game-reviews';
import { GameCompaniesComponent } from '../../components/game-companies/game-companies';
import { GameDetailsMediaComponent } from '../../components/game-details-media/game-details-media';
import { GameDiscoverMoreComponent } from '../../components/game-discover-more/game-discover-more';
import { GameHero } from '../../components/game-hero/game-hero';
import { GamePlayLinks } from '../../components/game-play-links/game-play-links';
import { GameQuickFacts } from '../../components/game-quick-facts/game-quick-facts';
import { GameDetailsResponse } from '../../models/games.interfaces';
import { GameService } from '../../services/games.service';

type GameDetailsStatus = 'loading' | 'ready' | 'error';

/**
 * Detalle de un juego (/gamevault/gamesdetails/:id).
 *
 * La página solo carga los datos y reparte cada parte a su componente:
 *  - game-hero:          cabecera, portada y acciones.
 *  - "Acerca de":         sinopsis y etiquetas (aquí mismo, es muy corto).
 *  - game-details-media: capturas y vídeos.
 *  - game-reviews:       reseñas de la comunidad.
 *  - game-quick-facts, game-play-links, game-discover-more y game-companies en la columna derecha.
 */
@Component({
  selector: 'app-game-details-page',
  imports: [
    RouterLink,
    GameHero,
    GameQuickFacts,
    GamePlayLinks,
    GameCompaniesComponent,
    GameDetailsMediaComponent,
    GameDiscoverMoreComponent,
    GameReviews,
  ],
  templateUrl: './game-details-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GameDetailsPage implements OnInit {
  private readonly gameService = inject(GameService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly status = signal<GameDetailsStatus>('loading');
  protected readonly gameDetails = signal<GameDetailsResponse | null>(null);
  protected readonly errorMessage = signal<string | null>(null);

  /** Pestañas de la barra fija: cada una baja hasta su sección. */
  protected readonly sectionTabs = [
    { id: 'overview', label: 'Acerca de' },
    { id: 'media', label: 'Capturas y vídeos' },
    { id: 'reviews', label: 'Reseñas' },
  ];

  protected readonly isLoading = computed(() => this.status() === 'loading');
  protected readonly displayErrorMessage = computed(
    () => this.errorMessage() ?? 'Ocurrió un error inesperado. Inténtalo de nuevo.',
  );

  /** Modos de juego y temas, sin repetir. */
  protected readonly detailChips = computed(() => {
    const game = this.gameDetails();
    return uniqueValues([...(game?.gameModes ?? []), ...(game?.themes ?? [])]);
  });

  /** Sinopsis y, si es distinta, la historia. */
  protected readonly aboutParagraphs = computed(() => {
    const game = this.gameDetails();
    const summary = normalizeText(game?.summary);
    const storyLine = normalizeText(game?.storyLine);
    const paragraphs = summary ? [summary] : [];

    if (storyLine && storyLine.toLocaleLowerCase() !== summary.toLocaleLowerCase()) {
      paragraphs.push(storyLine);
    }

    return paragraphs.length > 0 ? paragraphs : ['Aún no hay sinopsis disponible para este juego.'];
  });

  ngOnInit(): void {
    // Escuchamos los cambios del :id. Al navegar a un juego similar o a una
    // expansión, Angular reutiliza este componente y ngOnInit no se repite,
    // así que leer solo el snapshot dejaba los datos del juego anterior.
    this.route.paramMap.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((params) => {
      this.getGameDetails(Number(params.get('id')));
      window.scrollTo({ top: 0, behavior: 'smooth' });
    });
  }

  /** Baja hasta una sección del detalle (no usamos href="#id" porque el router lo trataría como otra ruta). */
  protected scrollToSection(id: string): void {
    document.getElementById(id)?.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }

  protected retryLoad(): void {
    this.getGameDetails(Number(this.route.snapshot.paramMap.get('id')));
  }

  private getGameDetails(gameId: number): void {
    this.status.set('loading');
    this.errorMessage.set(null);

    if (!gameId) {
      this.errorMessage.set('Ocurrió un error al cargar los detalles del juego...');
      setTimeout(() => {
        this.status.set('error');
        this.router.navigate(['/gamevault/games']);
      }, 2000);
      return;
    }

    this.gameService
      .getGameDetails(gameId)
      .pipe(take(1))
      .subscribe({
        next: (response) => {
          this.gameDetails.set(response);
          this.status.set('ready');
        },
        error: (error) => {
          this.errorMessage.set(errorMessages(error).join(' '));
          this.status.set('error');
        },
      });
  }
}
