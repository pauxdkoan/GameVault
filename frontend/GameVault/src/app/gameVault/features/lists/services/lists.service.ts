import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { PagedResult } from '../../../shared/models/api.models';
import { GameListDetail, GameListItem, GameListSummary, ListRequest } from '../models/list.models';

const API_URL = environment.gameVaultUrl;

@Injectable({ providedIn: 'root' })
export class ListsService {
  private readonly http = inject(HttpClient);

  /** Listas públicas de la comunidad (para el home). */
  getPublicLists(page = 1, pageSize = 6): Observable<PagedResult<GameListSummary>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedResult<GameListSummary>>(`${API_URL}/lists`, { params });
  }

  /** Mis listas, incluidas las privadas. */
  getMyLists(): Observable<GameListSummary[]> {
    return this.http.get<GameListSummary[]>(`${API_URL}/me/lists`);
  }

  getList(listId: string): Observable<GameListDetail> {
    return this.http.get<GameListDetail>(`${API_URL}/lists/${listId}`);
  }

  create(request: ListRequest): Observable<GameListSummary> {
    return this.http.post<GameListSummary>(`${API_URL}/lists`, request);
  }

  update(listId: string, request: ListRequest): Observable<GameListSummary> {
    return this.http.put<GameListSummary>(`${API_URL}/lists/${listId}`, request);
  }

  delete(listId: string): Observable<void> {
    return this.http.delete<void>(`${API_URL}/lists/${listId}`);
  }

  addGame(listId: string, externalId: number, note: string | null = null): Observable<GameListItem> {
    return this.http.post<GameListItem>(`${API_URL}/lists/${listId}/games/${externalId}`, { note });
  }

  removeGame(listId: string, externalId: number): Observable<void> {
    return this.http.delete<void>(`${API_URL}/lists/${listId}/games/${externalId}`);
  }
}
