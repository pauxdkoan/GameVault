import {
  HttpErrorResponse,
  HttpInterceptorFn,
  HttpRequest,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { AuthSessionService } from '../services/auth-session.service';
import {
  AuthService,
  PUBLIC_AUTH_URLS,
} from '../../features/auth/services/auth.service';

/** Añade "Authorization: Bearer <token>" a una petición. */
function withToken(request: HttpRequest<unknown>, token: string) {
  return request.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
}

/**
 * Interceptor de autenticación.
 *
 * 1. Si hay sesión, añade el access token a las peticiones a NUESTRA API.
 * 2. Si la API responde 401 (token caducado), pide uno nuevo con el
 *    refresh token y repite la petición UNA vez.
 * 3. Si la renovación falla, AuthService ya cerró la sesión local: el header
 *    pasa a "Iniciar sesión" y el authGuard protege las rutas privadas.
 */
export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const session = inject(AuthSessionService);
  const authService = inject(AuthService);

  const isOurApi = request.url.startsWith(environment.gameVaultUrl);
  const isPublicAuthUrl = PUBLIC_AUTH_URLS.some((url) =>
    request.url.startsWith(url),
  );

  if (!isOurApi || isPublicAuthUrl) {
    return next(request);
  }

  const token = session.accessToken();
  const authorizedRequest = token ? withToken(request, token) : request;

  return next(authorizedRequest).pipe(
    catchError((error: unknown) => {
      const isExpiredToken =
        error instanceof HttpErrorResponse && error.status === 401 && !!token;

      if (!isExpiredToken) {
        return throwError(() => error);
      }

      return authService
        .refreshAccessToken()
        .pipe(switchMap((newToken) => next(withToken(request, newToken))));
    }),
  );
};
