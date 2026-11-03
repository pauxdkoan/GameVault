export type GameSort = 'Popular' | 'Rating' | 'ReleaseDate';

export interface GameFilters {
  search?: string;
  genreIds?: number[];
  platformIds?: number[];
  sort?: GameSort;
}

export interface GetGamesRequest extends GameFilters {
  page: number;
  pageSize: number;
}

export interface SearchGamesRequest {
  search: string;
  page: number;
  pageSize: number;
}

export interface GetGameDetailsRequest {
  id: number;
}

export interface GetGamesResponse {
  count: number;
  page: number;
  pageSize: number;
  games: GameSummary[];
}

export interface GameSummary {
  externalId: number;
  shortSummary?: string | null;
  summary?: string | null;
  slug: string;
  name: string;
  trailerVideo: string | null;
  released: Date | string | null;
  screenshots: string[];
  coverImage?: string | null;
  rating: number;
  metacritic?: number | null;
  genres: string[];
  platforms: string[];
}

export interface GameDetailsResponse {
  externalId: number;
  summary: string | null;
  slug: string;
  name: string;
  released: Date | string | null;
  coverImage: string | null;
  rating: number;
  metacritic?: number | null;
  themes: string[];
  genres: string[];
  platforms: string[];
  videos: Video[];
  screenshots: string[];
  dlcs: Dlc[];
  expansions: Dlc[];
  expandedGames: Dlc[];
  stores: StoreLink[];
  gameModes: string[];
  companies: Company[];
  similarGames: Dlc[];
  storyLine: string | null;
  webSites: WebSite[];
  gameType: string | null;
  parentGame?: string | null;
  gameParent?: string | null;
}

export interface Company {
  name: string;
  logoImage: string | null;
  isDeveloper: boolean;
  isPublisher: boolean;
}

export interface Dlc {
  externalId: number;
  name: string;
  coverImage: string | null;
  raiting: number | null;
  platforms: string[];
  genres: string[];
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
