import { GameListSummary } from '../../lists/models/list.models';
import { Review } from '../../reviews/models/review.models';

/** Respuesta de GET /api/users/{userName}. */
export interface UserProfile {
  userName: string;
  displayName: string;
  bio: string | null;
  avatarUrl: string | null;
  bannerUrl: string | null;
  country: string | null;
  memberSince: string | null;
  isMe: boolean;
  stats: ProfileStats;
  recentReviews: Review[];
  publicLists: GameListSummary[];
}

export interface ProfileStats {
  gamesInLibrary: number;
  playing: number;
  completed: number;
  wishlist: number;
  reviews: number;
  publicLists: number;
}

/** Cuerpo de PUT /api/me/profile. */
export interface UpdateProfileRequest {
  displayName: string;
  bio: string | null;
  avatarUrl: string | null;
  bannerUrl: string | null;
  country: string | null;
}
