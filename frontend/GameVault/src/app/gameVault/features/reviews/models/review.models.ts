import { PagedResult, UserSummary } from '../../../shared/models/api.models';

export interface Review {
  id: string;
  gameExternalId: number;
  gameName: string;
  gameCoverImage: string | null;
  author: UserSummary;
  title: string;
  content: string;
  /** De 1 a 10. */
  rating: number;
  containsSpoilers: boolean;
  likeCount: number;
  commentCount: number;
  likedByMe: boolean;
  isMine: boolean;
  createdAt: string;
  updatedAt: string | null;
}

/** Respuesta de GET /api/games/{externalId}/reviews. */
export interface GameReviews {
  reviews: PagedResult<Review>;
  /** Media de 1 a 10, o null si no hay reseñas. */
  averageRating: number | null;
  reviewCount: number;
  myReview: Review | null;
}

export interface ReviewRequest {
  title: string;
  content: string;
  rating: number;
  containsSpoilers: boolean;
}

export interface ReviewComment {
  id: string;
  reviewId: string;
  author: UserSummary;
  content: string;
  isMine: boolean;
  createdAt: string;
}
