import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import {
  catchError,
  finalize,
  map,
  Observable,
  of,
  shareReplay,
  tap,
  throwError,
} from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { AuthSessionService } from '../../../core/services/auth-session.service';
import {
  LoginRequest,
  LoginResponse,
  SessionUser,
} from '../models/login.interfaces';
import {
  RegisterRequest,
  RegisterResponse,
} from '../models/register.interfaces';

const API_URL = environment.gameVaultUrl;

/** Rutas de auth que nunca deben llevar el access token ni reintentarse. */
export const PUBLIC_AUTH_URLS = [
  `${API_URL}/auth/login`,
  `${API_URL}/auth/register`,
  `${API_URL}/auth/refresh-tokens`,
  `${API_URL}/auth/logout`,
];

/**
 * Peticiones HTTP de autenticación.
 * Guarda el resultado en AuthSessionService; no toca localStorage directamente.
 */
@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly session = inject(AuthSessionService);

  /** Refresh en curso: si llegan varios 401 a la vez, todos esperan al mismo. */
  private refreshInFlight$: Observable<string> | null = null;

  login(loginRequest: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${API_URL}/auth/login`, loginRequest)
      .pipe(tap((response) => this.session.start(response)));
  }

  register(registerRequest: RegisterRequest): Observable<RegisterResponse> {
    // Si falla, error.interceptor.ts ya convierte el error en ApiError con sus mensajes.
    return this.http.post<RegisterResponse>(`${API_URL}/auth/register`, registerRequest);
  }

  /**
   * Pide un access token nuevo con el refresh token.
   * Devuelve el access token nuevo. Si falla, cierra la sesión local.
   */
  refreshAccessToken(): Observable<string> {
    if (this.refreshInFlight$) {
      return this.refreshInFlight$;
    }

    const refreshToken = this.session.refreshToken();

    if (!refreshToken) {
      this.session.clear();
      return throwError(() => new Error('Sin refresh token'));
    }

    this.refreshInFlight$ = this.http
      .post<LoginResponse>(`${API_URL}/auth/refresh-tokens`, { refreshToken })
      .pipe(
        tap((response) => this.session.start(response)),
        map((response) => response.accessToken),
        catchError((error) => {
          this.session.clear();
          return throwError(() => error);
        }),
        finalize(() => (this.refreshInFlight$ = null)),
        shareReplay(1),
      );

    return this.refreshInFlight$;
  }

  /** Recarga los datos del usuario desde el backend (GET /auth/me). */
  loadCurrentUser(): Observable<SessionUser | null> {
    if (!this.session.isLoggedIn()) {
      return of(null);
    }

    return this.http.get<SessionUser>(`${API_URL}/auth/me`).pipe(
      tap((user) => this.session.updateUser(user)),
      // Si falla, el interceptor ya intentó renovar; seguimos sin romper la app.
      catchError(() => of(null)),
    );
  }

  /**
   * Cierra la sesión. Se borra la sesión local SIEMPRE,
   * aunque el backend no responda.
   */
  logout(): Observable<void> {
    const refreshToken = this.session.refreshToken();
    this.session.clear();

    if (!refreshToken) {
      return of(undefined);
    }

    return this.http
      .post<void>(`${API_URL}/auth/logout`, { refreshToken })
      .pipe(catchError(() => of(undefined)));
  }
}
