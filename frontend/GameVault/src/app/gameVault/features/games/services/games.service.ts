import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, of, tap } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import {
  GameDetailsResponse,
  GameFiltersResponse,
  GetGamesRequest,
  GetGamesResponse,
  PageRequest,
} from '../models/games.interfaces';

const API_URL = `${environment.gameVaultUrl}/games`;

/**
 * Peticiones de juegos.
 *
 * Los listados se guardan en memoria (caché) para no repetir la misma
 * petición al volver al home o al catálogo. Los errores no se tratan aquí:
 * error.interceptor.ts los convierte en ApiError para todos los servicios.
 */
@Injectable({
  providedIn: 'root',
})
export class GameService {
  private readonly http = inject(HttpClient);
  private readonly cache = new Map<string, unknown>();

  /** GET /api/games: catálogo con búsqueda, filtros y orden. */
  getGames(request: GetGamesRequest): Observable<GetGamesResponse> {
    const params = this.buildGameParams(request);
    return this.getCached(`games?${params.toString()}`, () =>
      this.http.get<GetGamesResponse>(API_URL, { params }),
    );
  }

  /** GET /api/games/trending: mejor valorados de los últimos 6 meses. */
  getTrendingGames(request: PageRequest): Observable<GetGamesResponse> {
    const params = this.buildPageParams(request);
    return this.getCached(`trending?${params.toString()}`, () =>
      this.http.get<GetGamesResponse>(`${API_URL}/trending`, { params }),
    );
  }

  /** GET /api/games/upcoming: lanzamientos de los próximos 6 meses. */
  getUpcomingGames(request: PageRequest): Observable<GetGamesResponse> {
    const params = this.buildPageParams(request);
    return this.getCached(`upcoming?${params.toString()}`, () =>
      this.http.get<GetGamesResponse>(`${API_URL}/upcoming`, { params }),
    );
  }

  /** GET /api/games/filters: opciones de plataforma, género y orden. */
  getFilters(): Observable<GameFiltersResponse> {
    return this.getCached('filters', () =>
      this.http.get<GameFiltersResponse>(`${API_URL}/filters`),
    );
  }

  /**
   * GET /api/games/search: búsqueda por nombre.
   * La usan el buscador en vivo del header (6 sugerencias) y el catálogo
   * cuando hay texto en la caja (páginas de 20).
   * Se cachea por texto y página: al repetir una búsqueda no se pide otra vez.
   */
  searchGames(query: string, pageSize = 6, page = 1): Observable<GetGamesResponse> {
    const params = new HttpParams()
      .set('query', query)
      .set('page', page)
      .set('pageSize', pageSize);

    return this.getCached(`search?${params.toString()}`, () =>
      this.http.get<GetGamesResponse>(`${API_URL}/search`, { params }),
    );
  }

  /** GET /api/games/{id}: ficha completa. No se cachea para tener datos frescos. */
  getGameDetails(id: number): Observable<GameDetailsResponse> {
    return this.http.get<GameDetailsResponse>(`${API_URL}/${id}`);
  }

  /**
   * Devuelve la respuesta guardada para `key` o, si no existe,
   * hace la petición y guarda el resultado.
   * Antes este bloque estaba copiado en cada método.
   */
  private getCached<T>(key: string, request: () => Observable<T>): Observable<T> {
    if (this.cache.has(key)) {
      return of(this.cache.get(key) as T);
    }

    return request().pipe(tap((response) => this.cache.set(key, response)));
  }

  private buildPageParams(request: PageRequest): HttpParams {
    return new HttpParams()
      .set('page', request.page)
      .set('pageSize', request.pageSize);
  }

  private buildGameParams(request: GetGamesRequest): HttpParams {
    let params = this.buildPageParams(request);

    const search = request.search?.trim();

    if (search) {
      params = params.set('search', search);
    }

    for (const genreId of request.genreIds ?? []) {
      params = params.append('genreIds', genreId);
    }

    for (const platformId of request.platformIds ?? []) {
      params = params.append('platformIds', platformId);
    }

    if (request.sort !== undefined) {
      params = params.set('sort', request.sort);
    }

    return params;
  }
}
