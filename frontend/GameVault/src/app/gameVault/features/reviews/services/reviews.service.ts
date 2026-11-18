import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { PagedResult } from '../../../shared/models/api.models';
import { GameReviews, Review, ReviewComment, ReviewRequest } from '../models/review.models';

const API_URL = environment.gameVaultUrl;

/** Reseñas, me gusta y comentarios. */
@Injectable({ providedIn: 'root' })
export class ReviewsService {
  private readonly http = inject(HttpClient);

  getGameReviews(externalId: number, page = 1, pageSize = 10): Observable<GameReviews> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<GameReviews>(`${API_URL}/games/${externalId}/reviews`, { params });
  }

  create(externalId: number, request: ReviewRequest): Observable<Review> {
    return this.http.post<Review>(`${API_URL}/games/${externalId}/reviews`, request);
  }

  update(reviewId: string, request: ReviewRequest): Observable<Review> {
    return this.http.put<Review>(`${API_URL}/reviews/${reviewId}`, request);
  }

  delete(reviewId: string): Observable<void> {
    return this.http.delete<void>(`${API_URL}/reviews/${reviewId}`);
  }

  /** Devuelve el nuevo total de me gusta. */
  like(reviewId: string): Observable<number> {
    return this.http
      .post<{ likeCount: number }>(`${API_URL}/reviews/${reviewId}/like`, null)
      .pipe(map((response) => response.likeCount));
  }

  unlike(reviewId: string): Observable<number> {
    return this.http
      .delete<{ likeCount: number }>(`${API_URL}/reviews/${reviewId}/like`)
      .pipe(map((response) => response.likeCount));
  }

  getComments(reviewId: string, page = 1, pageSize = 50): Observable<PagedResult<ReviewComment>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedResult<ReviewComment>>(`${API_URL}/reviews/${reviewId}/comments`, { params });
  }

  addComment(reviewId: string, content: string): Observable<ReviewComment> {
    return this.http.post<ReviewComment>(`${API_URL}/reviews/${reviewId}/comments`, { content });
  }

  deleteComment(commentId: string): Observable<void> {
    return this.http.delete<void>(`${API_URL}/comments/${commentId}`);
  }
}
