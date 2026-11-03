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
  platformLabel: string[];
}
