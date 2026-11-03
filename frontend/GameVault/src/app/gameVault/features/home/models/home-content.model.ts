import { GameCard } from '../../../shared/models/game-card.model';

export interface HomeContent {
  heroSlides: HomeHeroSlide[];
  trendingGames: GameCard[];
  upcomingGames: GameCard[];
  discoverGames: GameCard[];
}

export interface HomeHeroSlide {
  id: string;
  externalId: number | null;
  name: string;
  slug: string;
  description: string | null;
  backgroundImageUrl: string | null;
  releaseDateLabel: string | null;
  rating: number | null;
  genres: string[];
}

/** Tarjeta de la sección "Mucho más que un catálogo". */
export interface HomeFeature {
  title: string;
  description: string;
  /** Trazo SVG (atributo d de <path>) del icono. */
  iconPath: string;
}

/** Actividad de un usuario en "Descubre lo que juega la comunidad". */
export interface CommunityActivity {
  id: string;
  userName: string;
  initials: string;
  action: string;
  target: string;
  timeAgo: string;
  quote: string | null;
  likes: number;
  comments: number;
}

/** Lista creada por un jugador en "Listas creadas por jugadores". */
export interface PlayerList {
  id: string;
  title: string;
  author: string;
  gameCount: number;
  followers: string;
}
