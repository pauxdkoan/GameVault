export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  /** Fecha ISO en la que caduca el access token. */
  accessTokenExpiresAt: string;
  user: SessionUser;
}

/** Portada que se muestra en el panel decorativo de login/registro. */
export interface ShowcaseCover {
  id: number;
  name: string;
  cover: string;
}

/** Usuario de la sesión (UserDto del backend). */
export interface SessionUser {
  id: string;
  email: string;
  userName: string;
  displayName: string;
  bio: string | null;
  avatarUrl: string | null;
  bannerUrl: string | null;
  country: string | null;
  isActive: boolean;
}
