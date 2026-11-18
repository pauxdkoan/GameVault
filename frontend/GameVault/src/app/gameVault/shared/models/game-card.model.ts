/** Familia de plataforma tal como la envía el backend (enum PlatformFamily). */
export type PlatformFamily =
  | 'PC'
  | 'PlayStation'
  | 'Xbox'
  | 'Nintendo'
  | 'Mobile'
  | 'VR'
  | 'Other';

/**
 * Tarjeta de juego tal como llega de la API (GameCardDto).
 * Es la misma para el catálogo, tendencias, próximos, similares, DLCs y expansiones.
 */
export interface GameCardDto {
  externalId: number;
  slug: string;
  name: string;
  summary: string | null;
  released: string | null;
  coverImage: string | null;
  screenshots: string[];
  trailerVideo: string | null;
  rating: number | null;
  metacritic: number | null;
  genres: string[];
  platforms: PlatformFamily[];
}

/** Tarjeta de juego lista para pintar en la vista (ver shared/mappers/game-card.mapper.ts). */
export interface GameCard {
  id: string;
  externalId?: number | null;
  name: string;
  slug: string;
  summary: string | null;
  releaseLabel: string;
  coverImageUrl: string | null;
  screenshotUrl: string | null;
  screenshotUrls: string[];
  trailerVideoUrl: string | null;
  rating: number;
  metacritic: number | null;
  genres: string[];
  /** Familias sin repetir ("PC", "PlayStation"...). */
  platforms: PlatformFamily[];
}
