import { GameCardDto, PlatformFamily } from '../../../shared/models/game-card.model';
import { LibraryEntry } from '../../library/models/library.models';

export type GameSort = 'Popular' | 'Rating' | 'ReleaseDate';

export interface GameFilters {
  search?: string;
  genreIds?: number[];
  platformIds?: number[];
  sort?: GameSort;
}

export interface PageRequest {
  page: number;
  pageSize: number;
}

export interface GetGamesRequest extends GameFilters, PageRequest {}

export interface GetGamesResponse {
  count: number;
  page: number;
  pageSize: number;
  games: GameCardDto[];
}

export interface GameDetailsResponse {
  externalId: number;
  summary: string | null;
  slug: string;
  name: string;
  released: Date | string | null;
  coverImage: string | null;
  rating: number | null;
  metacritic?: number | null;
  themes: string[];
  genres: string[];
  platforms: Platform[];
  videos: Video[];
  screenshots: string[];
  dlcs: GameCardDto[];
  expansions: GameCardDto[];
  expandedGames: GameCardDto[];
  stores: StoreLink[];
  gameModes: string[];
  companies: Company[];
  similarGames: GameCardDto[];
  storyLine: string | null;
  webSites: WebSite[];
  gameType: string | null;
  parentGame?: string | null;
  /** Nota media de las reseñas de GameVault (1-10), o null si no hay. */
  communityRating: number | null;
  reviewCount: number;
  /** El juego en mi biblioteca (solo con sesión), o null. */
  myLibraryEntry: LibraryEntry | null;
}

export interface Company {
  name: string;
  logoImage: string | null;
  isDeveloper: boolean;
  isPublisher: boolean;
}

/** Plataforma concreta del detalle ("PlayStation 5") y su familia. */
export interface Platform {
  name: string;
  family: PlatformFamily;
}

/** Respuesta de GET /api/games/filters. */
export interface GameFiltersResponse {
  platforms: FilterOption[];
  genres: FilterOption[];
  sorts: SortOption[];
}

export interface FilterOption {
  id: number;
  label: string;
}

export interface SortOption {
  value: GameSort;
  label: string;
}

export interface Video {
  name: string;
  url: string;
  embedUrl: string;
  thumbnailUrl: string;
}

export interface WebSite {
  name: string;
  url: string;
}

export interface StoreLink {
  storeName: string;
  url: string;
}
