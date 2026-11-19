import { HttpErrorResponse } from '@angular/common/http';

/** Mensaje cuando la API no responde (apagada, sin internet, CORS...). */
export const CONNECTION_ERROR = 'No pudimos conectar con el servidor. Inténtalo de nuevo.';

/** Mensaje cuando la API responde con error pero sin explicación. */
export const GENERIC_ERROR = 'Ocurrió un error inesperado. Inténtalo de nuevo.';

/**
 * Error de la API ya "traducido" para la vista.
 * Todas las peticiones que fallan llegan a los componentes como ApiError
 * (lo hace error.interceptor.ts), así que ningún componente tiene que
 * conocer el formato ProblemDetails del backend.
 */
export class ApiError extends Error {
  constructor(
    /** Código HTTP (0 si no hubo respuesta). */
    readonly status: number,
    /** Mensajes listos para mostrar. Siempre trae al menos uno. */
    readonly messages: string[],
  ) {
    super(messages.join(' '));
    this.name = 'ApiError';
  }
}

/**
 * Convierte un HttpErrorResponse en ApiError.
 *
 * El backend responde con ProblemDetails:
 *   - validación: { errors: { Campo: ["mensaje", ...] } }
 *   - resto:      { detail: "Credenciales inválidas." }
 */
export function toApiError(error: HttpErrorResponse): ApiError {
  if (error.status === 0) {
    return new ApiError(0, [CONNECTION_ERROR]);
  }

  // Errores del servidor (500, 502, 503...): su texto es técnico (por ejemplo
  // "No fue posible autenticar con IGDB"), así que al usuario solo se le
  // muestra el mensaje genérico, como haría una app en producción.
  if (error.status >= 500) {
    return new ApiError(error.status, [GENERIC_ERROR]);
  }

  const body = error.error;
  const validationMessages = Object.values(body?.errors ?? {})
    .flat()
    .filter((message): message is string => typeof message === 'string' && message !== '');

  if (validationMessages.length > 0) {
    return new ApiError(error.status, validationMessages);
  }

  const detail = body?.detail ?? body?.message;
  const message = typeof detail === 'string' && detail !== '' ? detail : GENERIC_ERROR;

  return new ApiError(error.status, [message]);
}

/** Para los `error:` de los subscribe: saca los mensajes de cualquier error. */
export function errorMessages(error: unknown): string[] {
  return error instanceof ApiError ? error.messages : [GENERIC_ERROR];
}
