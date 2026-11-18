import { Injectable, computed, signal } from '@angular/core';
import {
  LoginResponse,
  SessionUser,
} from '../../features/auth/models/login.interfaces';

/** Claves con las que se guarda la sesión en localStorage. */
const STORAGE_KEYS = {
  accessToken: 'gv.accessToken',
  refreshToken: 'gv.refreshToken',
  user: 'gv.user',
} as const;

/**
 * Guarda y expone la sesión del usuario.
 *
 * Es la ÚNICA pieza que toca localStorage. El resto de la app lee la
 * sesión desde aquí con signals:
 *   - user()        -> usuario actual o null
 *   - isLoggedIn()  -> true si hay sesión
 *
 * No hace peticiones HTTP: eso es trabajo de AuthService.
 */
@Injectable({ providedIn: 'root' })
export class AuthSessionService {
  private readonly accessTokenValue = signal<string | null>(
    this.read(STORAGE_KEYS.accessToken),
  );
  private readonly refreshTokenValue = signal<string | null>(
    this.read(STORAGE_KEYS.refreshToken),
  );
  private readonly userValue = signal<SessionUser | null>(this.readUser());

  readonly user = this.userValue.asReadonly();
  readonly isLoggedIn = computed(() => !!this.accessTokenValue());

  /** Iniciales para el avatar cuando el usuario no tiene foto. */
  readonly initials = computed(() => {
    const name = this.userValue()?.displayName || this.userValue()?.userName || '';
    return name
      .split(' ')
      .filter(Boolean)
      .slice(0, 2)
      .map((part) => part.charAt(0).toUpperCase())
      .join('');
  });

  accessToken(): string | null {
    return this.accessTokenValue();
  }

  refreshToken(): string | null {
    return this.refreshTokenValue();
  }

  /** Se llama tras un login o un refresh correcto. */
  start(response: LoginResponse): void {
    this.accessTokenValue.set(response.accessToken);
    this.refreshTokenValue.set(response.refreshToken);
    this.userValue.set(response.user);

    this.write(STORAGE_KEYS.accessToken, response.accessToken);
    this.write(STORAGE_KEYS.refreshToken, response.refreshToken);
    this.write(STORAGE_KEYS.user, JSON.stringify(response.user));
  }

  /** Actualiza solo los datos del usuario (por ejemplo, tras GET /auth/me). */
  updateUser(user: SessionUser): void {
    this.userValue.set(user);
    this.write(STORAGE_KEYS.user, JSON.stringify(user));
  }

  /** Borra todo rastro de la sesión. */
  clear(): void {
    this.accessTokenValue.set(null);
    this.refreshTokenValue.set(null);
    this.userValue.set(null);

    Object.values(STORAGE_KEYS).forEach((key) => this.remove(key));
  }

  private readUser(): SessionUser | null {
    const raw = this.read(STORAGE_KEYS.user);

    try {
      return raw ? (JSON.parse(raw) as SessionUser) : null;
    } catch {
      return null;
    }
  }

  // localStorage puede fallar (modo privado, almacenamiento lleno...):
  // en ese caso la sesión simplemente no se recuerda al recargar.
  private read(key: string): string | null {
    try {
      return localStorage.getItem(key);
    } catch {
      return null;
    }
  }

  private write(key: string, value: string): void {
    try {
      localStorage.setItem(key, value);
    } catch {
      /* sin persistencia */
    }
  }

  private remove(key: string): void {
    try {
      localStorage.removeItem(key);
    } catch {
      /* sin persistencia */
    }
  }
}
