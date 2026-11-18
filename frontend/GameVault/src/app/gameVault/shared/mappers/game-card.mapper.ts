import { GameCard, GameCardDto } from '../models/game-card.model';
import { formatReleaseDate } from '../utils/date';

/** Cuántos géneros se muestran como máximo en una tarjeta. */
const MAX_GENRES = 3;

/**
 * Convierte la tarjeta de la API (GameCardDto) en la tarjeta de la vista (GameCard).
 *
 * Antes había tres copias de esta lógica (catálogo, home y "descubre más")
 * y cada una tenía pequeñas diferencias. Ahora todas usan esta función.
 */
export function toGameCard(game: GameCardDto): GameCard {
  const screenshots = game.screenshots ?? [];
  const firstScreenshot = screenshots[0] ?? null;
  const genres = game.genres ?? [];

  return {
    id: game.externalId.toString(),
    externalId: game.externalId,
    name: game.name || 'Juego sin nombre',
    slug: game.slug || `game-${game.externalId}`,
    summary: game.summary ?? null,
    releaseLabel: formatReleaseDate(game.released),
    coverImageUrl: game.coverImage ?? firstScreenshot,
    screenshotUrl: firstScreenshot ?? game.coverImage ?? null,
    screenshotUrls: screenshots.length > 0 ? screenshots : toList(game.coverImage),
    trailerVideoUrl: game.trailerVideo ?? null,
    rating: game.rating ?? 0,
    metacritic: game.metacritic ?? null,
    genres: genres.length > 0 ? genres.slice(0, MAX_GENRES) : ['Sin género'],
    // Familias tal cual ("PC", "PlayStation"...): PlatformBadge elige el icono por familia.
    platforms: Array.from(new Set(game.platforms ?? [])),
  };
}

/** Atajo para mapear una lista entera. */
export function toGameCards(games: GameCardDto[] | null | undefined): GameCard[] {
  return (games ?? []).map(toGameCard);
}

function toList(value: string | null): string[] {
  return value ? [value] : [];
}
