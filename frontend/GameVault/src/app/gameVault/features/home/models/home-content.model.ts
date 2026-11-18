import { GameCard } from '../../../shared/models/game-card.model';

export interface HomeContent {
  heroSlides: HomeHeroSlide[];
  trendingGames: GameCard[];
  upcomingGames: GameCard[];
}

export interface HomeHeroSlide {
  id: string;
  externalId: number | null;
  name: string;
  slug: string;
  description: string | null;
  backgroundImageUrl: string | null;
  /** Portada vertical (2:3) del juego; es la imagen principal del carrusel. */
  coverImageUrl: string | null;
  releaseDateLabel: string | null;
  rating: number | null;
  genres: string[];
}

