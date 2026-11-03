export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAt: Date;
  user: User;
}

/** Portada que se muestra en el panel decorativo de login/registro. */
export interface ShowcaseCover {
  id: number;
  name: string;
  cover: string;
}

interface User {
  userName: string;
  displayName: string;
  bio: null;
  avatarUrl: null;
  bannerUrl: null;
  country: null;
  isActive: boolean;
}
