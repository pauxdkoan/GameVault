import { inject, Injectable } from '@angular/core';
import { GameService } from '../../games/services/games.service';
import { catchError, forkJoin, map, Observable, throwError } from 'rxjs';
import { HomeContent, HomeHeroSlide } from '../models/home-content.model';
import { GameCardDto } from '../../../shared/models/game-card.model';
import { toGameCards } from '../../../shared/mappers/game-card.mapper';
import { formatReleaseDate } from '../../../shared/utils/date';

@Injectable({
  providedIn: 'root',
})
export class HomeDataService {
  private readonly gameService = inject(GameService);

  getHomeContent(): Observable<HomeContent> {
    return forkJoin({
      trending: this.gameService.getTrendingGames({ page: 1, pageSize: 20 }),
      upcoming: this.gameService.getUpcomingGames({ page: 1, pageSize: 20 }),
    }).pipe(
      map(({ trending, upcoming }) => ({
        heroSlides: [
          ...trending.games.slice(0, 2).map((game) => this.toHeroSlide(game)),
          ...upcoming.games.slice(0, 2).map((game) => this.toHeroSlide(game)),
        ],
        trendingGames: toGameCards(trending.games),
        upcomingGames: toGameCards(upcoming.games),
      })),
      catchError((error) => {
        console.error('Error cargando el home', error);
        return throwError(
          () => new Error('No se pudo cargar el contenido del home.'),
        );
      }),
    );
  }

  /** El carrusel del hero usa un formato propio (imagen grande y descripción). */
  private toHeroSlide(game: GameCardDto): HomeHeroSlide {
    return {
      id: `hero-${game.externalId}`,
      externalId: game.externalId,
      name: game.name,
      slug: game.slug,
      description: game.summary ?? null,
      backgroundImageUrl: game.screenshots?.[0] ?? game.coverImage ?? null,
      coverImageUrl: game.coverImage ?? null,
      releaseDateLabel: formatReleaseDate(game.released),
      rating: game.rating ?? null,
      genres: game.genres ?? [],
    };
  }
}
