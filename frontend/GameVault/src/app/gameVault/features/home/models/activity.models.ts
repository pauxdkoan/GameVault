import { UserSummary } from '../../../shared/models/api.models';

export type ActivityType = 'Review' | 'Completed' | 'ListCreated';

/**
 * Un evento de la comunidad (GET /api/activity).
 * Según `type` vienen unos campos u otros:
 *  - Review: juego + reviewId, rating, excerpt, likeCount, commentCount.
 *  - Completed: juego.
 *  - ListCreated: listId, listName, listGameCount.
 */
export interface Activity {
  type: ActivityType;
  user: UserSummary;
  occurredAt: string;
  gameExternalId: number | null;
  gameName: string | null;
  gameCoverImage: string | null;
  reviewId: string | null;
  rating: number | null;
  excerpt: string | null;
  likeCount: number | null;
  commentCount: number | null;
  listId: string | null;
  listName: string | null;
  listGameCount: number | null;
}
