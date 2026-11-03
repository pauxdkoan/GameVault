import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, Observable, of, tap, throwError } from 'rxjs';
import { enviroment } from '../../../../../environments/enviroment';
import {
  GameDetailsResponse,
  GetGameDetailsRequest,
  GetGamesRequest,
  GetGamesResponse,
} from '../models/games.interfaces';

const API_URL = enviroment.gameVaultUrl;
@Injectable({
  providedIn: 'root',
})
export class GameService {
  private http = inject(HttpClient);
  private readonly cache = new Map<string, GetGamesResponse>();

  getGames(gameRequest: GetGamesRequest): Observable<GetGamesResponse> {
    const key = this.buildGamesCacheKey(gameRequest);
    const cached = this.cache.get(key);

    if (cached) {
      return of(cached);
    }

    return this.http
      .get<GetGamesResponse>(`${API_URL}/games/get-games`, {
        params: this.buildGameParams(gameRequest),
      })
      .pipe(
        tap((response) => {
          this.cache.set(key, response);
        }),
        catchError((error) => {
          const apiErrors = error.error?.errors;
          //Convierto el record en un array
          return throwError(() => Object.values(apiErrors));
        }),
      );
  }

  getTrendingGames(gameRequest: GetGamesRequest): Observable<GetGamesResponse> {
    const key = `trending:${gameRequest.page}:${gameRequest.pageSize}`;
    const cached = this.cache.get(key);

    if (cached) {
      return of(cached);
    }

    return this.http
      .get<GetGamesResponse>(`${API_URL}/games/get-trending-games`, {
        params: this.buildGameParams(gameRequest),
      })
      .pipe(
        tap((response) => {
          this.cache.set(key, response);
        }),
        catchError((error) => {
          const apiErrors = error.error?.errors;
          //Convierto el record en un array
          return throwError(() => Object.values(apiErrors));
        }),
      );
  }

  getUpCommingGames(
    gameRequest: GetGamesRequest,
  ): Observable<GetGamesResponse> {
    const key = `upcoming:${gameRequest.page}:${gameRequest.pageSize}`;
    const cached = this.cache.get(key);

    if (cached) {
      return of(cached);
    }

    return this.http
      .get<GetGamesResponse>(`${API_URL}/games/get-upcomming-games`, {
        params: this.buildGameParams(gameRequest),
      })
      .pipe(
        tap((response) => {
          this.cache.set(key, response);
        }),
        catchError((error) => {
          const apiErrors = error.error?.errors;
          //Convierto el record en un array
          return throwError(() => Object.values(apiErrors));
        }),
      );
  }

  getGameDetails(
    gameRequest: GetGameDetailsRequest,
  ): Observable<GameDetailsResponse> {
    return this.http
      .get<GameDetailsResponse>(`${API_URL}/games/get-game-details`, {
        params: { ...gameRequest },
      })
      .pipe(
        catchError((error) => {
          const apiErrors = error.error?.errors;
          //Convierto el record en un array
          return throwError(() => Object.values(apiErrors));
        }),
      );
  }

  private buildGamesCacheKey(request: GetGamesRequest): string {
    //Formato de como se guardaran las peticiones en cache para games
    return `games:${request.page}:${request.pageSize}:${request.search ?? ''}:${request.genreIds?.join(',') ?? ''}:${request.platformIds?.join(',') ?? ''}:${request.sort ?? ''}`;
  }
  private buildGameParams(gameRequest: GetGamesRequest): HttpParams {
    let params = new HttpParams()
      .set('page', gameRequest.page)
      .set('pageSize', gameRequest.pageSize);

    const search = gameRequest.search?.trim();

    if (search) {
      params = params.set('search', search);
    }

    for (const genreId of gameRequest.genreIds ?? []) {
      params = params.append('genreIds', genreId);
    }

    for (const platformId of gameRequest.platformIds ?? []) {
      params = params.append('platformIds', platformId);
    }

    if (gameRequest.sort !== undefined) {
      params = params.set('sort', gameRequest.sort);
    }

    return params;
  }
}
