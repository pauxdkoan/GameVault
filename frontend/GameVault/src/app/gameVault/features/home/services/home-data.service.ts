import { inject, Injectable } from '@angular/core';
import { GameService } from '../../games/services/games.service';
import { catchError, forkJoin, map, Observable, throwError } from 'rxjs';
import { HomeContent, HomeHeroSlide } from '../models/home-content.model';
import { GameSummary } from '../../games/models/games.interfaces';
import { GameCard } from '../../../shared/models/game-card.model';

@Injectable({
  providedIn: 'root',
})
export class HomeDataService {
  private readonly gameService = inject(GameService);

  getHomeContent(): Observable<HomeContent> {
    return forkJoin({
      trending: this.gameService.getTrendingGames({ page: 1, pageSize: 20 }),
      upcoming: this.gameService.getUpCommingGames({ page: 1, pageSize: 20 }),
      discover: this.gameService.getGames({ page: 1, pageSize: 20 }),
    }).pipe(
      map(({ trending, upcoming, discover }) => ({
        heroSlides: [
          ...trending.games
            .slice(0, 2)
            .map((game) => this.mapApiGameToHeroSlide(game)),
          ...upcoming.games
            .slice(0, 2)
            .map((game) => this.mapApiGameToHeroSlide(game)),
        ],
        trendingGames: trending.games.map((game) => this.mapApiGame(game)),
        upcomingGames: upcoming.games.map((game) => this.mapApiGame(game)),
        discoverGames: discover.games.map((game) => this.mapApiGame(game)),
      })),
      catchError((error) => {
        console.error('Error cargando el home', error);
        return throwError(
          () => new Error('No se pudo cargar el contenido del home.'),
        );
      }),
    );
  }

  private mapApiGame(game: GameSummary): GameCard {
    const platforms = game.platforms ?? [];
    const screenshots = game.screenshots ?? [];
    const screenshotUrl = screenshots[0] ?? null;

    return {
      id: game.externalId?.toString() ?? game.slug,
      externalId: game.externalId,
      name: game.name ?? 'Nombre no disponible',
      slug: game.slug ?? 'slug-no-disponible',
      summary: game.summary ?? null,
      releaseLabel: this.formatRelease(game.released ?? null),
      coverImageUrl: game.coverImage ?? screenshotUrl,
      rating: game.rating ?? 0,
      metacritic: game.metacritic ?? null,
      genres: game.genres ?? [],
      platformLabel: game.platforms ?? [],
      screenshotUrl: screenshotUrl ?? game.coverImage ?? null,
      screenshotUrls: screenshots,
      trailerVideoUrl: game.trailerVideo ?? null,
    };
  }

  private mapApiGameToHeroSlide(game: GameSummary): HomeHeroSlide {
    const screenshotUrl = game.screenshots?.[0] ?? null;

    return {
      id: `hero-${game.externalId ?? game.slug}`,
      externalId: game.externalId ?? null,
      name: game.name,
      slug: game.slug,
      description: game.summary ?? null,
      backgroundImageUrl: screenshotUrl ?? game.coverImage ?? null,
      releaseDateLabel: this.formatRelease(game.released),
      rating: game.rating ?? null,
      genres: game.genres ?? [],
    };
  }

  private formatRelease(released: Date | string | null): string {
    if (released === null) {
      return 'Fecha por confirmar';
    }

    const releaseDate = new Date(released);

    if (Number.isNaN(releaseDate.getTime())) {
      return 'Fecha por confirmar';
    }

    return new Intl.DateTimeFormat('es', {
      day: 'numeric',
      month: 'short',
      year: 'numeric',
    }).format(releaseDate);
  }
}
