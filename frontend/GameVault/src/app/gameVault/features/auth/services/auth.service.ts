import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { LoginRequest, LoginResponse } from '../models/login.interfaces';
import { catchError, Observable, throwError } from 'rxjs';
import { enviroment } from '../../../../../environments/enviroment';
import {
  RegisterRequest,
  RegisterResponse,
} from '../models/register.interfaces';

const API_URL = enviroment.gameVaultUrl;

/** Mensaje cuando la API no responde o el error no trae detalle. */
const GENERIC_ERROR = 'No pudimos conectar con el servidor. Inténtalo de nuevo.';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);

  login(loginRequest: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${API_URL}/auth/login`, loginRequest)
      .pipe(catchError((error) => throwError(() => this.toMessages(error))));
  }

  register(registerRequest: RegisterRequest): Observable<RegisterResponse> {
    return this.http
      .post<RegisterResponse>(`${API_URL}/auth/register`, registerRequest)
      .pipe(catchError((error) => throwError(() => this.toMessages(error))));
  }

  /**
   * Convierte cualquier error HTTP en una lista de textos para la vista.
   *
   * La API devuelve { errors: { Campo: ["mensaje", ...] } } o
   * { errors: ["mensaje"] }. Si no viene nada (API caída, CORS...)
   * devolvemos un mensaje genérico en vez de romper.
   */
  private toMessages(error: HttpErrorResponse): string[] {
    const apiErrors = error.error?.errors;

    if (!apiErrors) {
      return [error.error?.message ?? GENERIC_ERROR];
    }

    const messages = Object.values(apiErrors)
      .flat()
      .filter((message): message is string => typeof message === 'string');

    return messages.length > 0 ? messages : [GENERIC_ERROR];
  }
}
