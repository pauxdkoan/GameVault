import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { toApiError } from '../http/api-error';

/**
 * Interceptor de errores global.
 *
 * Convierte cualquier HttpErrorResponse en ApiError (con mensajes listos
 * para mostrar). Antes cada servicio tenía su propio catchError, y en
 * GameService fallaba si la respuesta no traía "errors" (Object.values(undefined)).
 *
 * Va el PRIMERO en withInterceptors: así recibe la respuesta el último,
 * después de que authInterceptor haya intentado renovar el token ante un 401.
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) =>
  next(req).pipe(
    catchError((error: unknown) => {
      if (error instanceof HttpErrorResponse) {
        return throwError(() => toApiError(error));
      }

      return throwError(() => error);
    }),
  );
